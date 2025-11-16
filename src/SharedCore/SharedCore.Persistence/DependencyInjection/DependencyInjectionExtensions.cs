using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SharedCore.Persistence.DependencyInjection;

/// <summary>
/// The dependency injection extensions.
/// </summary>
[ExcludeFromCodeCoverage]
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Adds the shared persistence dependencies.
    /// </summary>
    /// <typeparam name="TContext">The specific type of DB context.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="isToAddDefaultDatabaseProvider">Value indicating whether it should add the default database provider. If set to false, configure the context by overriding the OnConfiguring(DbContextOptionsBuilder) method in your derived context.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddSharedPersistence<TContext>(this IServiceCollection services,
        IConfiguration configuration, bool isToAddDefaultDatabaseProvider = true)
        where TContext : DbContext
    {
        services.AddSharedEfCore<TContext>(configuration, isToAddDefaultDatabaseProvider);

        services.AddSharedRepositories(configuration);

        return services;
    }

    /// <summary>
    /// Adds the shared persistence health checks.
    /// </summary>
    /// <param name="healthChecksBuilder">The health checks builder.</param>
    public static IHealthChecksBuilder AddSharedPersistenceHealthChecks<TContext>(
        this IHealthChecksBuilder healthChecksBuilder)
        where TContext : DbContext
    {
        healthChecksBuilder.AddSharedEfCoreHealthChecks<TContext>();

        return healthChecksBuilder;
    }
}