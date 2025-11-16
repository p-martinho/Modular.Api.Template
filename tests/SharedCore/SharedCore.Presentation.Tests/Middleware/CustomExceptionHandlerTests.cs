using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SharedCore.Presentation.Middleware;
using SharedCore.Presentation.Tests.TestHelpers;

namespace SharedCore.Presentation.Tests.Middleware;

public class CustomExceptionHandlerTests
{
    private const string EndpointThatThrows = "/exception";

    private readonly MockLogger<CustomExceptionHandler> _logger;

    public CustomExceptionHandlerTests()
    {
        _logger = Substitute.For<MockLogger<CustomExceptionHandler>>();
    }

    [Fact]
    public async Task TryHandleAsync_WhenNotBadHttpRequestException_ShouldLogAndWriteProblemDetails()
    {
        // Arrange
        var exception = new Exception("ExceptionMessage");
        using var host = await BuildHostAsync(exception);

        // Act
        var response = await host.GetTestClient().GetAsync(EndpointThatThrows, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        _logger.Received(1).LogError(exception,
            $"An unexpected error occurred while processing the request: {exception.Message}");
        var problemDetails =
            await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);
        Assert.NotNull(problemDetails);
        Assert.Equal(StatusCodes.Status500InternalServerError, problemDetails.Status);
        Assert.Equal("An unexpected internal error occurred.", problemDetails.Detail);
    }

    [Fact]
    public async Task TryHandleAsync_WhenBadHttpRequestException_ShouldWriteProblemDetailsAndNotLog()
    {
        // Arrange
        var exception = new BadHttpRequestException("BadHttpRequestExceptionMessage");
        using var host = await BuildHostAsync(exception);

        // Act
        var response = await host.GetTestClient().GetAsync(EndpointThatThrows, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        _logger.DidNotReceiveWithAnyArgs().LogError(null!, null!);
        var problemDetails =
            await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);
        Assert.NotNull(problemDetails);
        Assert.Equal(StatusCodes.Status400BadRequest, problemDetails.Status);
        Assert.Equal("The request is invalid.", problemDetails.Detail);
    }

    private Task<IHost> BuildHostAsync(Exception exceptionToThrow)
    {
        return new HostBuilder()
            .ConfigureWebHost(webBuilder =>
            {
                webBuilder
                    .UseTestServer()
                    .ConfigureServices(services =>
                    {
                        services.AddProblemDetails();
                        services.AddExceptionHandler<CustomExceptionHandler>();
                        services.AddRouting();
                        services.AddScoped<ILogger<CustomExceptionHandler>>(_ => _logger);
                    })
                    .Configure(app =>
                    {
                        app.UseRouting();
                        app.UseExceptionHandler();
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapGet(EndpointThatThrows, _ => throw exceptionToThrow);
                        });
                    });
            })
            .StartAsync();
    }
}