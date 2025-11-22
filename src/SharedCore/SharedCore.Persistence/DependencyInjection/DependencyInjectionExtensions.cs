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
    /// The <see cref="IServiceCollection"/> extensions.
    /// </summary>
    /// <param name="services">The service collection.</param>
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Adds the shared persistence dependencies.
        /// </summary>
        /// <typeparam name="TContext">The specific type of DB context.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <param name="isToAddDefaultDatabaseProvider">
        /// Value indicating whether it should add the default database provider.
        /// If set to false, configure the context by overriding the OnConfiguring(DbContextOptionsBuilder) method in your derived context.
        /// </param>
        /// <returns>The service collection.</returns>
        public IServiceCollection AddSharedPersistence<TContext>(IConfiguration configuration,
            bool isToAddDefaultDatabaseProvider = true) where TContext : DbContext
        {
            services.AddSharedEfCore<TContext>(configuration, isToAddDefaultDatabaseProvider);

            services.AddSharedRepositories(configuration);

            return services;
        }
    }

    /// <summary>
    /// The <see cref="IHealthChecksBuilder"/> extensions.
    /// </summary>
    /// <param name="healthChecksBuilder">The health checks builder.</param>
    extension(IHealthChecksBuilder healthChecksBuilder)
    {
        /// <summary>
        /// Adds the shared persistence health checks.
        /// </summary>
        public IHealthChecksBuilder AddSharedPersistenceHealthChecks<TContext>() where TContext : DbContext
        {
            healthChecksBuilder.AddSharedEfCoreHealthChecks<TContext>();

            return healthChecksBuilder;
        }
    }
}