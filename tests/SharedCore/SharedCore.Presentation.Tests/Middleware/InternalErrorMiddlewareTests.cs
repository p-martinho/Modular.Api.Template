using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using NSubstitute;
using SharedCore.Presentation.Middleware;
using SharedCore.Presentation.Settings;
using SharedCore.Presentation.Tests.TestHelpers;

namespace SharedCore.Presentation.Tests.Middleware;

public class InternalErrorMiddlewareTests
{
    private readonly InternalErrorMiddleware _middleware;
    private readonly MockLogger<InternalErrorMiddleware> _logger;
    private readonly InternalErrorMiddlewareSettings _settings;

    public InternalErrorMiddlewareTests()
    {
        _logger = Substitute.For<MockLogger<InternalErrorMiddleware>>();

        _settings = new InternalErrorMiddlewareSettings();

        _middleware = BuildMiddleware(StatusCodes.Status500InternalServerError, _settings);
    }

    [Fact]
    public async Task InvokeAsync_WhenInternalError_ShouldLogTheRequest()
    {
        // Arrange
        const string bodyContent = "RequestBody";
        const string queryStringKey = "RequestQueryStringKey";
        const string queryStringValue = "RequestQueryStringValue";
        const string requestPath = "/test.com/api/test";
        const string requestMethod = "POST";
        var requestBodyBytes = Encoding.UTF8.GetBytes(bodyContent);
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Method = requestMethod;
        httpContext.Request.Path = requestPath;
        httpContext.Request.QueryString = httpContext.Request.QueryString.Add(queryStringKey, queryStringValue);
        httpContext.Request.Body = new MemoryStream(requestBodyBytes);
        httpContext.Request.ContentLength = requestBodyBytes.Length;

        // Act
        await _middleware.InvokeAsync(httpContext);

        // Assert
        _logger.Received(1)
            .LogError(Arg.Is<string>(s =>
                s.Contains($"{requestMethod} {requestPath}?{queryStringKey}={queryStringValue}") &&
                s.Contains($"Body: {bodyContent}")));
    }

    [Fact]
    public async Task InvokeAsync_WhenInternalErrorWithEmptyBody_ShouldNotLogBody()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();

        // Act
        await _middleware.InvokeAsync(httpContext);

        // Assert
        _logger.Received(1)
            .LogError(Arg.Is<string>(s => !s.Contains("Body:")));
    }

    [Fact]
    public async Task InvokeAsync_WhenInternalErrorAndBodyExceedsMaxLength_ShouldLogBodyTruncated()
    {
        // Arrange
        var bodyContent = new string('A', _settings.MaxBodyLengthInLog + 8);
        var requestBodyBytes = Encoding.UTF8.GetBytes(bodyContent);
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Body = new MemoryStream(requestBodyBytes);
        httpContext.Request.ContentLength = requestBodyBytes.Length;

        // Act
        await _middleware.InvokeAsync(httpContext);

        // Assert
        var truncatedBody = bodyContent[.._settings.MaxBodyLengthInLog];
        _logger.Received(1)
            .LogError(Arg.Is<string>(s => s.Contains($"Body: {truncatedBody} ...")));
    }

    [Fact]
    public async Task InvokeAsync_WhenInternalErrorAndExceptionReadingTheBody_ShouldLogError()
    {
        // Arrange
        const string bodyContent = "RequestBody";
        var requestBodyBytes = Encoding.UTF8.GetBytes(bodyContent);
        var httpContext = new DefaultHttpContext { Request = { Body = new MemoryStream(requestBodyBytes) } };
        InternalErrorMiddlewareSettings settings = null!; // Will cause an exception on read
        var middleware = BuildMiddleware(StatusCodes.Status500InternalServerError, settings);

        // Act
        await middleware.InvokeAsync(httpContext);

        // Assert
        _logger.Received(1)
            .LogError(Arg.Is<string>(s => s.Contains("Error reading the request body")));
    }

    [Fact]
    public async Task InvokeAsync_WhenNoInternalError_ShouldNotLog()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        var middleware = BuildMiddleware(StatusCodes.Status400BadRequest, _settings);

        // Act
        await middleware.InvokeAsync(httpContext);

        // Assert
        _logger.DidNotReceiveWithAnyArgs()
            .LogError(null!);
    }

    private InternalErrorMiddleware BuildMiddleware(int statusCodeFromNext, InternalErrorMiddlewareSettings settings)
    {
        RequestDelegate next = context =>
        {
            context.Response.StatusCode = statusCodeFromNext;
            return Task.CompletedTask;
        };

        return new InternalErrorMiddleware(next, _logger, Options.Create(settings));
    }
}