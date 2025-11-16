using System.Diagnostics.CodeAnalysis;
using ModuleTemplate.Application.DependencyInjection;
using SharedCore.Presentation.DependencyInjection;

namespace ModuleTemplate.Presentation.Api.DependencyInjection;

/// <summary>
/// The dependency injection extensions.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class DependencyInjectionExtensions
{
    /// <summary>
    /// Adds the custom health checks dependencies.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add here specific health checks for this API. Default health checks were already registered in ServiceDefaults project.

        services.AddHealthChecks()
            .AddApplicationHealthChecks(configuration);

        return services;
    }

    /// <summary>
    /// Adds the API dependencies.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="hostEnvironment">The host environment.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddApiDependencies(this IServiceCollection services, IConfiguration configuration,
        IHostEnvironment hostEnvironment)
    {
        services.AddSharedPresentation(configuration, hostEnvironment);

        services.AddApplication(configuration);

        return services;
    }
}