using System.Security.Claims;
using Identity.Application.Commands.Tokens.Exchange;
using Identity.Presentation.Api.Endpoints.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using OpenIddict.Abstractions;
using OpenIddict.Server;
using OpenIddict.Server.AspNetCore;
using SharedCore.Application.Commands.Models;

namespace Identity.Presentation.Api.IntegrationTests.Endpoints;

/// <remarks>
/// These tests are complementary to the integration tests, to test scenarios not possible to test in the integration tests (for instance, internal errors).
/// </remarks>
public class TokensEndpointGroupTests
{
    [Fact]
    public async Task ExchangeAsync_WhenInternalError_ShouldReturnProblemDetails()
    {
        // Arrange
        var httpContext = GetMockedHttpContext();
        var commandHandler = Substitute.For<IExchangeTokenCommandHandler>();
        commandHandler.HandleAsync(null!, CancellationToken.None)
            .ReturnsForAnyArgs(CommandOut<ClaimsPrincipal>.InternalError());

        // Act
        var result = await TokensEndpointGroup.ExchangeAsync(httpContext,
            commandHandler, TestContext.Current.CancellationToken);

        // Assert
        var problemHttpResult = result.Result as ProblemHttpResult;
        Assert.NotNull(problemHttpResult);
        Assert.Equal(StatusCodes.Status500InternalServerError, problemHttpResult.StatusCode);
        Assert.Equal(StatusCodes.Status500InternalServerError, problemHttpResult.ProblemDetails.Status);
    }

    [Fact]
    public async Task ExchangeAsync_WhenInvalidRequest_ShouldReturnProblemDetails()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        var commandHandler = Substitute.For<IExchangeTokenCommandHandler>();

        // Act
        var result = await TokensEndpointGroup.ExchangeAsync(httpContext,
            commandHandler, TestContext.Current.CancellationToken);

        // Assert
        var problemHttpResult = result.Result as ProblemHttpResult;
        Assert.NotNull(problemHttpResult);
        Assert.Equal(StatusCodes.Status400BadRequest, problemHttpResult.StatusCode);
        Assert.Equal(StatusCodes.Status400BadRequest, problemHttpResult.ProblemDetails.Status);
        await commandHandler.DidNotReceiveWithAnyArgs().HandleAsync(null!, CancellationToken.None);
    }

    [Fact]
    public async Task ExchangeAsync_WhenAuthorizationError_ShouldReturnForbid()
    {
        // Arrange
        var httpContext = GetMockedHttpContext();
        var commandHandler = Substitute.For<IExchangeTokenCommandHandler>();
        commandHandler.HandleAsync(null!, CancellationToken.None)
            .ReturnsForAnyArgs(CommandOut<ClaimsPrincipal>.AuthorizationError());

        // Act
        var result = await TokensEndpointGroup.ExchangeAsync(httpContext,
            commandHandler, TestContext.Current.CancellationToken);

        // Assert
        var problemHttpResult = result.Result as ForbidHttpResult;
        Assert.NotNull(problemHttpResult);
    }

    [Fact]
    public async Task ExchangeAsync_WhenNoData_ShouldReturnProblemDetails()
    {
        // Arrange
        var httpContext = GetMockedHttpContext();
        var commandHandler = Substitute.For<IExchangeTokenCommandHandler>();
        commandHandler.HandleAsync(null!, CancellationToken.None)
            .ReturnsForAnyArgs(CommandOut<ClaimsPrincipal>.Success(null!));

        // Act
        var result = await TokensEndpointGroup.ExchangeAsync(httpContext,
            commandHandler, TestContext.Current.CancellationToken);

        // Assert
        var problemHttpResult = result.Result as ProblemHttpResult;
        Assert.NotNull(problemHttpResult);
        Assert.Equal(StatusCodes.Status500InternalServerError, problemHttpResult.StatusCode);
        Assert.Equal(StatusCodes.Status500InternalServerError, problemHttpResult.ProblemDetails.Status);
    }

    private static HttpContext GetMockedHttpContext()
    {
        var httpContext = Substitute.For<HttpContext>();

        httpContext.Features.Get<OpenIddictServerAspNetCoreFeature>().Returns(new OpenIddictServerAspNetCoreFeature
        {
            Transaction = new OpenIddictServerTransaction { Request = new OpenIddictRequest() }
        });

        return httpContext;
    }
}