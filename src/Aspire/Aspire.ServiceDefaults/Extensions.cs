using System.Diagnostics.CodeAnalysis;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using SharedCore.Common.Authorization;
using SharedCore.Common.HealthChecks;

namespace Aspire.ServiceDefaults;

// Adds common .NET Aspire services: service discovery, resilience, health checks, and OpenTelemetry.
// This project should be referenced by each service project in your solution.
// To learn more about using this project, see https://aka.ms/dotnet/aspire/service-defaults
/// <summary>
/// The service defaults extensions.
/// </summary>
[ExcludeFromCodeCoverage]
public static class Extensions
{
    private const string TimeoutPolicyForHealthChecksName = "HealthChecks";
    private const string OutputCachePolicyForHealthChecksName = "HealthChecks";
    private const int DefaultHealthChecksTimeoutInSeconds = 5;
    private const int DefaultCacheExpirationInSeconds = 10;

    /// <summary>
    /// Adds service defaults.
    /// </summary>
    /// <param name="builder">The Host application builder.</param>
    /// <typeparam name="TBuilder">The type of the Host application builder.</typeparam>
    /// <returns>The Host application builder.</returns>
    public static TBuilder AddServiceDefaults<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.ConfigureOpenTelemetry();

        builder.AddDefaultHealthChecks();

        builder.Services.AddServiceDiscovery();

        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            // Turn on resilience by default
            http.AddStandardResilienceHandler();

            // Turn on service discovery by default
            http.AddServiceDiscovery();
        });

        // Uncomment the following to restrict the allowed schemes for service discovery.
        // builder.Services.Configure<ServiceDiscoveryOptions>(options =>
        // {
        //     options.AllowedSchemes = ["https"];
        // });

        builder.Services.AddRequestTimeouts();
        builder.Services.AddOutputCache();

        return builder;
    }

    /// <summary>
    /// Configures the OpenTelemetry.
    /// </summary>
    /// <param name="builder">The Host application builder.</param>
    /// <typeparam name="TBuilder">The type of the Host application builder.</typeparam>
    /// <returns>The Host application builder.</returns>
    public static TBuilder ConfigureOpenTelemetry<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
        });

        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();
            })
            .WithTracing(tracing =>
            {
                if (builder.Environment.IsDevelopment())
                {
                    tracing.SetSampler<AlwaysOnSampler>();
                }

                tracing.AddSource(builder.Environment.ApplicationName)
                    .AddAspNetCoreInstrumentation(t =>
                        // Exclude health check requests from tracing
                        t.Filter = context =>
                            !context.Request.Path.StartsWithSegments(HealthChecksEndpoints.HealthEndpointPath)
                            && !context.Request.Path.StartsWithSegments(HealthChecksEndpoints.AlivenessEndpointPath)
                    )
                    // Uncomment the following line to enable gRPC instrumentation (requires the OpenTelemetry.Instrumentation.GrpcNetClient package)
                    //.AddGrpcClientInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation();
            });

        builder.AddOpenTelemetryExporters();

        return builder;
    }

    /// <summary>
    /// Adds the default health checks.
    /// </summary>
    /// <param name="builder">The Host application builder.</param>
    /// <typeparam name="TBuilder">The type of the Host application builder.</typeparam>
    /// <returns>The Host application builder.</returns>
    public static TBuilder AddDefaultHealthChecks<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        // Configures request timeouts and output caching for these endpoints to prevent abuse or denial-of-service attacks.

        var healthChecksTimeout = GetHealthChecksTimeoutInSeconds(builder.Configuration);

        if (healthChecksTimeout > 0)
        {
            builder.Services.AddRequestTimeouts(
                configure: timeouts =>
                    timeouts.AddPolicy(TimeoutPolicyForHealthChecksName, TimeSpan.FromSeconds(healthChecksTimeout)));
        }

        var healthChecksCacheExpiration = GetHealthChecksCacheExpirationInSeconds(builder.Configuration);

        if (healthChecksCacheExpiration > 0)
        {
            builder.Services.AddOutputCache(
                configureOptions: caching =>
                    caching.AddPolicy(OutputCachePolicyForHealthChecksName,
                        build: policy => policy.Expire(TimeSpan.FromSeconds(healthChecksCacheExpiration))));
        }

        builder.Services.AddHealthChecks()
            // Add a default aliveness check to ensure app is responsive
            .AddCheck("Self", () => HealthCheckResult.Healthy(), [HealthChecksTags.Live]);

        return builder;
    }

    /// <summary>
    /// Maps the default endpoints.
    /// </summary>
    /// <param name="app">The Web application.</param>
    /// <returns>The Web application.</returns>
    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        app.UseRequestTimeouts();
        app.UseOutputCache();

        var healthChecksGroup = app.MapGroup(string.Empty);

        healthChecksGroup
            .WithRequestTimeout(TimeoutPolicyForHealthChecksName)
            .CacheOutput(OutputCachePolicyForHealthChecksName);

        // All health checks must pass for app to be considered ready to accept traffic after starting
        healthChecksGroup.MapHealthChecks(HealthChecksEndpoints.HealthEndpointPath);

        healthChecksGroup.MapHealthChecks(HealthChecksEndpoints.FullHealthEndpointPath,
                new HealthCheckOptions {ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse})
            .RequireAuthorization(Policies.HealthChecksFull);

        // Only health checks tagged with the "live" tag must pass for app to be considered alive
        app.MapHealthChecks(HealthChecksEndpoints.AlivenessEndpointPath,
            new HealthCheckOptions {Predicate = r => r.Tags.Contains(HealthChecksTags.Live)});

        return app;
    }

    private static void AddOpenTelemetryExporters<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

        if (useOtlpExporter)
        {
            builder.Services.AddOpenTelemetry().UseOtlpExporter();
        }

        // Uncomment the following lines to enable the Azure Monitor exporter (requires the Azure.Monitor.OpenTelemetry.AspNetCore package)
        //if (!string.IsNullOrEmpty(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]))
        //{
        //    builder.Services.AddOpenTelemetry()
        //       .UseAzureMonitor();
        //}
    }

    private static int GetHealthChecksTimeoutInSeconds(IConfiguration configuration)
    {
        var isParseSuccessful = int.TryParse(configuration.GetSection("HealthChecks:TimeoutInSeconds").Value,
            out var timeout);

        return isParseSuccessful ? timeout : DefaultHealthChecksTimeoutInSeconds;
    }

    private static int GetHealthChecksCacheExpirationInSeconds(IConfiguration configuration)
    {
        var isParseSuccessful = int.TryParse(configuration.GetSection("HealthChecks:CacheExpirationInSeconds").Value,
            out var cacheExpiration);

        return isParseSuccessful ? cacheExpiration : DefaultCacheExpirationInSeconds;
    }
}