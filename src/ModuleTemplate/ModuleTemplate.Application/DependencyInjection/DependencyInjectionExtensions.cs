using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ModuleTemplate.Persistence.DependencyInjection;
using SharedCore.Application.DependencyInjection;

namespace ModuleTemplate.Application.DependencyInjection;

/// <summary>
/// The dependency injection extensions.
/// </summary>
[ExcludeFromCodeCoverage]
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Adds the application dependencies.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSharedApplication(configuration);

        services.AddPersistence(configuration);

        services.AddValidators();

        services.AddCommandHandlers();

        services.AddQueryHandlers();

        return services;
    }

    /// <summary>
    /// Adds the application health checks.
    /// </summary>
    /// <param name="healthChecksBuilder">The health checks builder.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The health checks builder.</returns>
    public static IHealthChecksBuilder AddApplicationHealthChecks(this IHealthChecksBuilder healthChecksBuilder,
        IConfiguration configuration)
    {
        healthChecksBuilder.AddPersistenceHealthChecks(configuration);

        return healthChecksBuilder;
    }

    private static void AddValidators(this IServiceCollection services)
    {
    }

    private static void AddCommandHandlers(this IServiceCollection services)
    {
    }

    private static void AddQueryHandlers(this IServiceCollection services)
    {
    }
}