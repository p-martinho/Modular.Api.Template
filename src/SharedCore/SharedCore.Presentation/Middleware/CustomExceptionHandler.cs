using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SharedCore.Presentation.Middleware;

/// <summary>
/// The custom exception handler.
/// </summary>
/// <seealso cref="IExceptionHandler"/>
public class CustomExceptionHandler : IExceptionHandler
{
    private readonly ILogger<CustomExceptionHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomExceptionHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public CustomExceptionHandler(ILogger<CustomExceptionHandler> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        // BadHttpRequestException: exception thrown on binding errors.
        var isBadRequestException = exception is BadHttpRequestException;

        var problemDetails = new ProblemDetails
        {
            Status = isBadRequestException
                ? StatusCodes.Status400BadRequest
                : StatusCodes.Status500InternalServerError,
            Detail = isBadRequestException
                ? "The request is invalid."
                : "An unexpected internal error occurred."
        };

        if (!isBadRequestException)
        {
            _logger.LogError(exception,
                "An unexpected error occurred while processing the request: {ExceptionMessage}",
                exception.Message);
        }

        await Results.Problem(problemDetails).ExecuteAsync(httpContext);

        // true: the exception was handled, stop processing.
        // false: control falls back to the default behavior and options from the middleware.
        return true;
    }
}