using System.Security.Claims;
using Identity.Application.Commands.OpenId.Seed;
using Identity.Domain.Entities.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using SharedCore.Application.Commands;
using SharedCore.Application.Commands.Models;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Identity.Application.Commands.Tokens.Exchange;

/// <summary>
/// The exchange client token command handler.
/// </summary>
/// <seealso cref="CommandHandler{TIn,TOutData}"/>
/// <seealso cref="IExchangeTokenCommandHandler"/>
internal class ExchangeTokenCommandHandler : CommandHandler<OpenIddictRequest, ClaimsPrincipal>,
    IExchangeTokenCommandHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<AppIdentityUser> _userManager;
    private readonly SignInManager<AppIdentityUser> _signInManager;
    private readonly IOpenIddictScopeManager _scopeManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="SeedOpenIdTestingResourcesCommandHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="httpContextAccessor">The current user.</param>
    /// <param name="userManager">The user manager.</param>
    /// <param name="signInManager">The sign in manager.</param>
    /// <param name="scopeManager">The OpenId scope manager.</param>
    public ExchangeTokenCommandHandler(ILogger<ExchangeTokenCommandHandler> logger,
        IHttpContextAccessor httpContextAccessor,
        UserManager<AppIdentityUser> userManager,
        SignInManager<AppIdentityUser> signInManager,
        IOpenIddictScopeManager scopeManager)
        : base(logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
        _signInManager = signInManager;
        _scopeManager = scopeManager;
    }

    /// <inheritdoc />
    protected override Task<CommandOut<ClaimsPrincipal>> HandleCommandInAsync(OpenIddictRequest commandIn,
        CancellationToken cancellationToken)
    {
        if (commandIn.IsAuthorizationCodeGrantType() || commandIn.IsRefreshTokenGrantType())
        {
            return HandleAccessTokenAsync(cancellationToken);
        }

        if (commandIn.IsPasswordGrantType())
        {
            return HandlePasswordGrantAsync(commandIn, cancellationToken);
        }

        return Task.FromResult(CommandOut<ClaimsPrincipal>.ValidationError("The specified grant is not supported."));
    }

    private async Task<CommandOut<ClaimsPrincipal>> HandleAccessTokenAsync(CancellationToken cancellationToken)
    {
        var userId = await GetUserIdFromAuthorizationCodeOrRefreshToken();

        if (userId is null)
        {
            return CommandOut<ClaimsPrincipal>.AuthorizationError("The token is no longer valid.");
        }

        // Retrieve the user profile corresponding to the authorization code/refresh token.
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return CommandOut<ClaimsPrincipal>.AuthorizationError("The token is no longer valid.");
        }

        // Ensure the user is still allowed to sign in.
        if (!await _signInManager.CanSignInAsync(user))
        {
            return CommandOut<ClaimsPrincipal>.AuthorizationError("The user is no longer allowed to sign in.");
        }

        // Note: the scopes are automatically defined from the authorization code or refresh token.
        var identity = await BuildClaimsIdentityAsync(user, [], cancellationToken);

        return CommandOut<ClaimsPrincipal>.Success(new ClaimsPrincipal(identity));
    }

    private async Task<string?> GetUserIdFromAuthorizationCodeOrRefreshToken()
    {
        if (_httpContextAccessor.HttpContext is null)
        {
            return null;
        }

        // Retrieve the claims principal stored in the authorization code/refresh token.
        var authenticationResult = await _httpContextAccessor.HttpContext
            .AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        return authenticationResult.Succeeded ? authenticationResult.Principal?.GetClaim(Claims.Subject) : null;
    }

    private async Task<CommandOut<ClaimsPrincipal>> HandlePasswordGrantAsync(OpenIddictRequest commandIn,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(commandIn.Username!);

        if (user == null)
        {
            return CommandOut<ClaimsPrincipal>.AuthorizationError("The username/password couple is invalid.");
        }

        // Validate the username/password parameters and ensure the account is not locked out.
        var result = await _signInManager.CheckPasswordSignInAsync(user, commandIn.Password!, lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            return CommandOut<ClaimsPrincipal>.AuthorizationError("The username/password couple is invalid.");
        }

        var identity = await BuildClaimsIdentityAsync(user, [.. commandIn.GetScopes()], cancellationToken);

        return CommandOut<ClaimsPrincipal>.Success(new ClaimsPrincipal(identity));
    }

    private async Task<ClaimsIdentity> BuildClaimsIdentityAsync(AppIdentityUser currentUser, string[] scopes,
        CancellationToken cancellationToken)
    {
        // Create the claims-based identity that will be used by OpenIddict to generate tokens.
        var identity = new ClaimsIdentity(await _userManager.GetClaimsAsync(currentUser),
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: Claims.Name,
            roleType: Claims.Role);

        // Add the claims that will be persisted in the tokens.
        identity.SetClaim(Claims.Subject, currentUser.Id)
            .SetClaim(Claims.Email, currentUser.Email)
            .SetClaim(Claims.Name, currentUser.UserName)
            .SetClaim(Claims.PreferredUsername, currentUser.UserName)
            .SetClaims(Claims.Role, [.. await _userManager.GetRolesAsync(currentUser)]);

        if (scopes.Length != 0)
        {
            // Set the list of scopes granted to the client application.
            identity.SetScopes(scopes);
            identity.SetResources(await _scopeManager.ListResourcesAsync(identity.GetScopes(), cancellationToken)
                .ToListAsync(cancellationToken));
        }

        identity.SetDestinations(GetDestinations);

        return identity;
    }

    private static IEnumerable<string> GetDestinations(Claim claim)
    {
        // Note: by default, claims are NOT automatically included in the access and identity tokens.
        // To allow OpenIddict to serialize them, you must attach them a destination, that specifies
        // whether they should be included in access tokens, in identity tokens or in both.

        // "sub" claim is required and doesn't need to specify a destination, it will be included in the access token anyway.

        return claim.Type switch
        {
            // Allow the "name" claim to be stored in both the access and identity tokens
            // when the "profile" scope was granted (by calling principal.SetScopes(...)).
            Claims.Name or Claims.PreferredUsername when HasScope(claim, Scopes.Profile) =>
                [Destinations.AccessToken, Destinations.IdentityToken],
            Claims.Email when HasScope(claim, Scopes.Email) =>
                [Destinations.AccessToken, Destinations.IdentityToken],
            Claims.Role when HasScope(claim, Scopes.Roles) =>
                [Destinations.AccessToken, Destinations.IdentityToken],
            // Otherwise, only store the claim in the access tokens.
            _ => [Destinations.AccessToken]
        };
    }

    private static bool HasScope(Claim claim, string scope)
    {
        return claim.Subject?.HasScope(scope) ?? false;
    }
}