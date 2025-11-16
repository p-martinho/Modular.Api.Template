using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using SharedCore.Presentation.Middleware;
using SharedCore.Presentation.Settings;

namespace SharedCore.Presentation.Extensions;

/// <summary>
/// The Web application extensions.
/// </summary>
[ExcludeFromCodeCoverage]
public static class WebApplicationExtensions
{
    /// <summary>
    /// Adds the internal error middleware into the Web application.
    /// </summary>
    /// <param name="app">The Web application.</param>
    /// <returns>The Web application.</returns>
    public static WebApplication UseInternalErrorMiddleware(this WebApplication app)
    {
        if (IsInternalErrorMiddlewareEnabled(app.Configuration))
        {
            app.UseMiddleware<InternalErrorMiddleware>();
        }

        return app;
    }

    private static bool IsInternalErrorMiddlewareEnabled(IConfiguration configuration)
    {
        var settings = configuration.GetSection(nameof(InternalErrorMiddlewareSettings))
            .Get<InternalErrorMiddlewareSettings>();

        return settings?.IsEnabled ?? false;
    }
}