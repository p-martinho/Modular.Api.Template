using System.Security.Claims;
using Identity.Application.Commands.Tokens.Exchange;
using Identity.Domain.Entities.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using OpenIddict.Abstractions;
using SharedCore.Application.Common.Models;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Identity.Application.Tests.Commands.Tokens;

public class ExchangeTokenCommandHandlerTests
{
    private readonly ExchangeTokenCommandHandler _commandHandler;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<AppIdentityUser> _userManager;
    private readonly SignInManager<AppIdentityUser> _signInManager;

    public ExchangeTokenCommandHandlerTests()
    {
        _httpContextAccessor = Substitute.For<IHttpContextAccessor>();

        _userManager = Substitute.For<UserManager<AppIdentityUser>>(
            Substitute.For<IUserStore<AppIdentityUser>>(),
            Substitute.For<IOptions<IdentityOptions>>(),
            Substitute.For<IPasswordHasher<AppIdentityUser>>(),
            Substitute.For<IEnumerable<IUserValidator<AppIdentityUser>>>(),
            Substitute.For<IEnumerable<IPasswordValidator<AppIdentityUser>>>(),
            Substitute.For<ILookupNormalizer>(),
            Substitute.For<IdentityErrorDescriber>(),
            Substitute.For<IServiceProvider>(),
            Substitute.For<ILogger<UserManager<AppIdentityUser>>>());

        _signInManager = Substitute.For<SignInManager<AppIdentityUser>>(
            _userManager,
            _httpContextAccessor,
            Substitute.For<IUserClaimsPrincipalFactory<AppIdentityUser>>(),
            Substitute.For<IOptions<IdentityOptions>>(),
            Substitute.For<ILogger<SignInManager<AppIdentityUser>>>(),
            Substitute.For<IAuthenticationSchemeProvider>(),
            Substitute.For<IUserConfirmation<AppIdentityUser>>());

        var scopeManager = Substitute.For<IOpenIddictScopeManager>();

        _commandHandler = new ExchangeTokenCommandHandler(NullLogger<ExchangeTokenCommandHandler>.Instance,
            _httpContextAccessor,
            _userManager,
            _signInManager,
            scopeManager);
    }

    [Fact]
    public async Task HandleAsync_WhenRefreshTokenGrant_ShouldSucceed()
    {
        // Arrange
        const string userId = "userId";
        var commandIn = new OpenIddictRequest { GrantType = GrantTypes.RefreshToken };
        var authService = Substitute.For<IAuthenticationService>();
        _httpContextAccessor.HttpContext!.RequestServices.GetService(typeof(IAuthenticationService))
            .Returns(authService);
        authService.AuthenticateAsync(null!, null).ReturnsForAnyArgs(BuildSuccessAuthenticateResult(userId));
        var user = AppIdentityUser.Create("Email", "FirstName", "LastName")!;
        user.Id = userId;
        user.UserName = user.Email;
        _userManager.FindByIdAsync(userId).Returns(user);
        _signInManager.CanSignInAsync(user).Returns(true);
        Claim[] userClaims = [new("SomeClaimType1", "SomeClaimValue1"), new("SomeClaimType2", "SomeClaimValue2")];
        IList<string> userRoles = ["SomeRole1", "SomeRole2"];
        _userManager.GetClaimsAsync(user).Returns(userClaims);
        _userManager.GetRolesAsync(user).Returns(userRoles);

        // Act
        var result = await _commandHandler.HandleAsync(commandIn, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.Success, result.Result.ResultType);
        Assert.NotNull(result.Data);
        var claimsInResult = result.Data.Claims.ToList();
        Assert.Contains(userClaims,
            userClaim => claimsInResult.Any(c => c.Type == userClaim.Type && c.Value == userClaim.Value));
        Assert.Contains(userRoles, userRole => claimsInResult.Any(c => c.Type == Claims.Role && c.Value == userRole));
        Assert.Contains(claimsInResult, claim => claim.Type == Claims.Subject && claim.Value == userId);
        Assert.Contains(claimsInResult, claim => claim.Type == Claims.Email && claim.Value == user.Email);
        Assert.Contains(claimsInResult, claim => claim.Type == Claims.Name && claim.Value == user.UserName);
        Assert.Contains(claimsInResult,
            claim => claim.Type == Claims.PreferredUsername && claim.Value == user.UserName);
    }

    [Fact]
    public async Task HandleAsync_WhenRefreshTokenGrant_AndHttpContextIsNull_ShouldReturnAuthorizationError()
    {
        // Arrange
        var commandIn = new OpenIddictRequest { GrantType = GrantTypes.RefreshToken };
        _httpContextAccessor.HttpContext.Returns((HttpContext?)null);

        // Act
        var result = await _commandHandler.HandleAsync(commandIn, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.AuthorizationError, result.Result.ResultType);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task HandleAsync_WhenRefreshTokenGrant_AndAuthenticationFailed_ShouldReturnAuthorizationError()
    {
        // Arrange
        var commandIn = new OpenIddictRequest { GrantType = GrantTypes.RefreshToken };
        var authService = Substitute.For<IAuthenticationService>();
        _httpContextAccessor.HttpContext!.RequestServices.GetService(typeof(IAuthenticationService))
            .Returns(authService);
        authService.AuthenticateAsync(null!, null).ReturnsForAnyArgs(AuthenticateResult.Fail("Authentication Failed"));

        // Act
        var result = await _commandHandler.HandleAsync(commandIn, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.AuthorizationError, result.Result.ResultType);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task HandleAsync_WhenRefreshTokenGrant_AndUserNotFound_ShouldReturnAuthorizationError()
    {
        // Arrange
        const string userId = "userId";
        var commandIn = new OpenIddictRequest { GrantType = GrantTypes.RefreshToken };
        var authService = Substitute.For<IAuthenticationService>();
        _httpContextAccessor.HttpContext!.RequestServices.GetService(typeof(IAuthenticationService))
            .Returns(authService);
        authService.AuthenticateAsync(null!, null).ReturnsForAnyArgs(BuildSuccessAuthenticateResult(userId));
        _userManager.FindByIdAsync(null!).ReturnsForAnyArgs((AppIdentityUser?)null);

        // Act
        var result = await _commandHandler.HandleAsync(commandIn, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.AuthorizationError, result.Result.ResultType);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task HandleAsync_WhenRefreshTokenGrant_AndUserCantSignIn_ShouldReturnAuthorizationError()
    {
        // Arrange
        const string userId = "userId";
        var commandIn = new OpenIddictRequest { GrantType = GrantTypes.RefreshToken };
        var authService = Substitute.For<IAuthenticationService>();
        _httpContextAccessor.HttpContext!.RequestServices.GetService(typeof(IAuthenticationService))
            .Returns(authService);
        authService.AuthenticateAsync(null!, null).ReturnsForAnyArgs(BuildSuccessAuthenticateResult(userId));
        var user = AppIdentityUser.Create("Email", null, null)!;
        _userManager.FindByIdAsync(null!).ReturnsForAnyArgs(user);
        _signInManager.CanSignInAsync(null!).ReturnsForAnyArgs(false);

        // Act
        var result = await _commandHandler.HandleAsync(commandIn, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.AuthorizationError, result.Result.ResultType);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task HandleAsync_WhenPasswordGrant_ShouldSucceed()
    {
        // Arrange
        const string userId = "userId";
        var commandIn = new OpenIddictRequest
        {
            GrantType = GrantTypes.Password,
            Username = "Email",
            Password = "Password",
            Scope = $"{Scopes.Profile} {Scopes.Email} {Scopes.Roles}"
        };
        var user = AppIdentityUser.Create("Email", "FirstName", "LastName")!;
        user.Id = userId;
        user.UserName = user.Email;
        _userManager.FindByNameAsync(commandIn.Username!).Returns(user);
        _signInManager.CheckPasswordSignInAsync(user, commandIn.Password!, lockoutOnFailure: true)
            .Returns(SignInResult.Success);
        Claim[] userClaims = [new("SomeClaimType1", "SomeClaimValue1"), new("SomeClaimType2", "SomeClaimValue2")];
        IList<string> userRoles = ["SomeRole1", "SomeRole2"];
        _userManager.GetClaimsAsync(user).Returns(userClaims);
        _userManager.GetRolesAsync(user).Returns(userRoles);

        // Act
        var result = await _commandHandler.HandleAsync(commandIn, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.Success, result.Result.ResultType);
        Assert.NotNull(result.Data);
        var claimsInResult = result.Data.Claims.ToList();
        Assert.Contains(userClaims,
            userClaim => claimsInResult.Any(c => c.Type == userClaim.Type && c.Value == userClaim.Value));
        Assert.Contains(userRoles, userRole => claimsInResult.Any(c => c.Type == Claims.Role && c.Value == userRole));
        Assert.Contains(claimsInResult, claim => claim.Type == Claims.Subject && claim.Value == userId);
        Assert.Contains(claimsInResult, claim => claim.Type == Claims.Email && claim.Value == user.Email);
        Assert.Contains(claimsInResult, claim => claim.Type == Claims.Name && claim.Value == user.UserName);
        Assert.Contains(claimsInResult,
            claim => claim.Type == Claims.PreferredUsername && claim.Value == user.UserName);
        Assert.NotEmpty(result.Data.GetScopes());
    }

    [Fact]
    public async Task HandleAsync_WhenPasswordGrant_AndUserNotFound_ShouldReturnAuthorizationError()
    {
        // Arrange
        var commandIn = new OpenIddictRequest { GrantType = GrantTypes.Password };
        _userManager.FindByNameAsync(null!).ReturnsForAnyArgs((AppIdentityUser?)null);

        // Act
        var result = await _commandHandler.HandleAsync(commandIn, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.AuthorizationError, result.Result.ResultType);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task HandleAsync_WhenPasswordGrant_AndInvalidPassword_ShouldReturnAuthorizationError()
    {
        // Arrange
        var commandIn = new OpenIddictRequest { GrantType = GrantTypes.Password };
        var user = AppIdentityUser.Create("Email", null, null)!;
        _userManager.FindByNameAsync(null!).ReturnsForAnyArgs(user);
        _signInManager.CheckPasswordSignInAsync(null!, null!, false)
            .ReturnsForAnyArgs(SignInResult.Failed);

        // Act
        var result = await _commandHandler.HandleAsync(commandIn, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.AuthorizationError, result.Result.ResultType);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task HandleAsync_WhenUnsupportedGrant_ShouldReturnValidationError()
    {
        // Arrange
        var commandIn = new OpenIddictRequest();

        // Act
        var result = await _commandHandler.HandleAsync(commandIn, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.ValidationError, result.Result.ResultType);
        Assert.Equal("The specified grant is not supported.", result.Result.Message);
        Assert.Null(result.Data);
    }

    private static AuthenticateResult BuildSuccessAuthenticateResult(string userId)
    {
        Claim[] claims = [new(Claims.Subject, userId)];

        var identity = new ClaimsIdentity(claims);

        var principal = new ClaimsPrincipal(identity);

        var ticket = new AuthenticationTicket(principal, "TestScheme");

        return AuthenticateResult.Success(ticket);
    }
}