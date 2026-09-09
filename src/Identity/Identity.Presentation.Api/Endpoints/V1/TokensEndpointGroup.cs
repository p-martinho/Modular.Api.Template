using Asp.Versioning;
using Asp.Versioning.Builder;
using Identity.Application.Commands.Tokens.Exchange;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Server.AspNetCore;
using SharedCore.Application.Common.Models;
using SharedCore.Presentation.Endpoints;
using SharedCore.Presentation.MappingExtensions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Identity.Presentation.Api.Endpoints.V1;

/// <summary>
/// The tokens endpoint group.
/// </summary>
/// <seealso cref="IEndpointGroup"/>
internal sealed class TokensEndpointGroup : IEndpointGroup
{
    private const string EndpointGroupName = "Tokens";

    private static readonly ApiVersion ApiVersion = new(1, 0);

    /// <inheritdoc />
    public static void Map(IVersionedEndpointRouteBuilder apiBuilder)
    {
        var group = apiBuilder.MapGroup("")
            .HasApiVersion(ApiVersion)
            .WithTags(EndpointGroupName);

        group.MapPost("connect/token", ExchangeAsync)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);
    }

    /// <summary>
    /// Gets an access token.
    /// </summary>
    /// <param name="httpContext">The HTTP context.</param>
    /// <param name="commandHandler">The command handler.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response result.</returns>
    public static async Task<Results<SignInHttpResult, ForbidHttpResult, ProblemHttpResult>>
        ExchangeAsync(HttpContext httpContext, IExchangeTokenCommandHandler commandHandler,
            CancellationToken cancellationToken)
    {
        var request = httpContext.GetOpenIddictServerRequest();

        if (request is null)
        {
            return TypedResults.Problem(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Detail = "Please provide a valid OpenID Connect request."
            });
        }

        var commandOut = await commandHandler.HandleAsync(request, cancellationToken);

        if (commandOut.Result.ResultType == ResultType.AuthorizationError)
        {
            // Need to return Forbid to the OpenIddict middleware handle it.
            return BuildForbidResult(commandOut.Result.Message);
        }

        if (!commandOut.Result.IsSuccess)
        {
            return TypedResults.Problem(commandOut.Result.ToProblemDetails());
        }

        if (commandOut.Data is null)
        {
            // The result is success, but Data is null, something went wrong.
            return TypedResults.Problem(new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Detail = "Unexpected error occurred."
            });
        }

        // Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens.
        return TypedResults.SignIn(commandOut.Data,
            authenticationScheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    private static ForbidHttpResult BuildForbidResult(string? errorDescription)
    {
        return TypedResults.Forbid(
            authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme],
            properties: new AuthenticationProperties(new Dictionary<string, string?>
            {
                [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = errorDescription
            }));
    }
}