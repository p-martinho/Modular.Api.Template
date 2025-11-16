using System.Diagnostics.CodeAnalysis;
using Asp.Versioning;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Validation.AspNetCore;
using Serilog;
using SharedCore.Common.Authorization;
using SharedCore.Common.Extensions;
using SharedCore.Presentation.Middleware;
using SharedCore.Presentation.Settings;

namespace SharedCore.Presentation.DependencyInjection;

/// <summary>
/// The dependency injection extensions.
/// </summary>
[ExcludeFromCodeCoverage]
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Adds the shared presentation dependencies.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="hostEnvironment">The host environment.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddSharedPresentation(this IServiceCollection services,
        IConfiguration configuration, IHostEnvironment hostEnvironment)
    {
        services.AddHttpContextAccessor();

        services.AddSettings(configuration);

        services.AddProblemDetails();

        services.AddExceptionHandler<CustomExceptionHandler>();

        services.AddAuthenticationAndAuthorization(configuration, hostEnvironment);

        services.AddApiVersioning();

        services.AddLogging(configuration);

        return services;
    }

    private static void AddSettings(this IServiceCollection services, IConfiguration configuration)
    {
        // Throw exception on binding error (on non-Development) (the exception handler will handle it)
        services.Configure<RouteHandlerOptions>(options => { options.ThrowOnBadRequest = true; });

        services.Configure<InternalErrorMiddlewareSettings>(
            configuration.GetSection(nameof(InternalErrorMiddlewareSettings)));
    }

    private static void AddProblemDetails(this IServiceCollection services)
    {
        services.AddProblemDetails(options =>
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Instance =
                    $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";

                context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);

                var activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
                context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
            });
    }

    private static void AddAuthenticationAndAuthorization(this IServiceCollection services,
        IConfiguration configuration, IHostEnvironment hostEnvironment)
    {
        services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);

        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.HealthChecksFull,
                policyBuilder => policyBuilder.RequireRole(UserRoles.Admin).Build());
        });

        services.AddOpenIddictValidation(configuration, hostEnvironment);
    }

    private static void AddOpenIddictValidation(this IServiceCollection services,
        IConfiguration configuration, IHostEnvironment hostEnvironment)
    {
        var isToDisableDefaultConfig =
            configuration.GetSection("IdentitySettings:DisableDefaultValidationConfiguration").Get<bool>();

        // This way, each service can disable this default configuration and then configure it by itself.
        if (isToDisableDefaultConfig)
        {
            return;
        }

        services.AddOpenIddict()
            .AddValidation(options =>
            {
                // Note: the validation handler uses OpenID Connect discovery
                // to retrieve the issuer signing keys used to validate tokens.
                options.SetIssuer(configuration["IdentitySettings:Issuer"] ?? string.Empty);

                options.AddAudiences(configuration["IdentitySettings:Audience"] ?? string.Empty);

                if (!hostEnvironment.IsDevelopment() && !hostEnvironment.IsMigration())
                {
                    // Register the encryption credentials.
                    options.AddEncryptionKey(new SymmetricSecurityKey(
                        Convert.FromBase64String(configuration["IdentitySettings:EncryptionKey"] ?? string.Empty)));
                }

                // Register the System.Net.Http integration.
                options.UseSystemNetHttp();

                // Register the ASP.NET Core host.
                options.UseAspNetCore();
            });
    }

    private static void AddApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VV";
            });
    }

    private static void AddLogging(this IServiceCollection services, IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Async(cfg => cfg.Console())
            .CreateLogger();

        services.AddSerilog(loggerConfig =>
        {
            loggerConfig.ReadFrom.Configuration(configuration);
            loggerConfig.WriteTo.OpenTelemetry();
        });
    }
}