using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedCore.Presentation.Settings;

namespace SharedCore.Presentation.Middleware;

/// <summary>
/// The internal error middleware.
/// </summary>
public class InternalErrorMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<InternalErrorMiddleware> _logger;
    private readonly InternalErrorMiddlewareSettings _settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="InternalErrorMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next request delegate.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="options">The options.</param>
    public InternalErrorMiddleware(RequestDelegate next,
        ILogger<InternalErrorMiddleware> logger,
        IOptions<InternalErrorMiddlewareSettings> options)
    {
        _next = next;
        _logger = logger;
        _settings = options.Value;
    }

    /// <summary>
    /// Invokes the middleware asynchronous.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        // Enable buffering, to be able to reset the position
        context.Request.EnableBuffering();

        // Call the next delegate/middleware in the pipeline.
        await _next(context);

        // Handle the response
        if (context.Response.StatusCode == StatusCodes.Status500InternalServerError)
        {
            var requestBody = await GetRequestBodyAsStringAsync(context.Request);

            LogInternalError(context.Request, requestBody);
        }
    }

    private async Task<string?> GetRequestBodyAsStringAsync(HttpRequest request)
    {
        try
        {
            return await ReadRequestBodyAsync(request);
        }
        catch (Exception e)
        {
            return $"(Error reading the request body: {e.Message})";
        }
    }

    private async Task<string?> ReadRequestBodyAsync(HttpRequest request)
    {
        // Reset position
        request.Body.Position = 0;

        const int bufferSize = 1024;
        var buffer = new byte[bufferSize];
        var totalLength = 0;
        var stringBuilder = new StringBuilder();

        // Read in chunks (in case of a large body is unnecessary to read it all)
        while (true)
        {
            var bytesRead = await request.Body.ReadAsync(buffer.AsMemory(0, bufferSize));
            if (bytesRead == 0)
            {
                break;
            }

            totalLength += bytesRead;

            stringBuilder.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));

            if (totalLength > _settings.MaxBodyLengthInLog)
            {
                break;
            }
        }

        // Reset position, for subsequence readings
        request.Body.Position = 0;

        var bodyAsString = stringBuilder.ToString();

        if (!string.IsNullOrEmpty(bodyAsString) && bodyAsString.Length > _settings.MaxBodyLengthInLog)
        {
            return $"{bodyAsString[.._settings.MaxBodyLengthInLog]} ...";
        }

        return bodyAsString;
    }

    private void LogInternalError(HttpRequest request, string? requestBodyAsString)
    {
        if (string.IsNullOrWhiteSpace(requestBodyAsString))
        {
            _logger.LogError(
                "Internal server error for request with parameters: {RequestMethod} {RequestPath}{RequestQueryString}",
                request.Method, request.Path, request.QueryString);
        }
        else
        {
            _logger.LogError(
                "Internal server error for request with parameters: {RequestMethod} {RequestPath}{RequestQueryString}\nBody: {RequestBody}",
                request.Method, request.Path, request.QueryString, requestBodyAsString);
        }
    }
}