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
using Microsoft.Extensions.Logging.Testing;
using SharedCore.Presentation.Middleware;

namespace SharedCore.Presentation.Tests.Middleware;

public class CustomExceptionHandlerTests
{
    private const string EndpointThatThrows = "/exception";

    private readonly FakeLogger<CustomExceptionHandler> _fakeLogger;

    public CustomExceptionHandlerTests()
    {
        _fakeLogger = new FakeLogger<CustomExceptionHandler>();
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
        Assert.Equal(LogLevel.Error, _fakeLogger.LatestRecord.Level);
        Assert.Equal(exception, _fakeLogger.LatestRecord.Exception);
        Assert.Contains($"An unexpected error occurred while processing the request: {exception.Message}",
            _fakeLogger.LatestRecord.Message);
        Assert.Equal(LogLevel.Error, _fakeLogger.LatestRecord.Level);
        Assert.Equal(exception, _fakeLogger.LatestRecord.Exception);
        Assert.Contains(exception.Message, _fakeLogger.LatestRecord.Message);
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
        Assert.Empty(_fakeLogger.Collector.GetSnapshot());
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
                        services.AddScoped<ILogger<CustomExceptionHandler>>(_ => _fakeLogger);
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