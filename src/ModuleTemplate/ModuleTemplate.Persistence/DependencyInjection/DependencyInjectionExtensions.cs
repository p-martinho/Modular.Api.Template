using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ModuleTemplate.Common.DependencyInjection;
using SharedCore.Persistence.DependencyInjection;

namespace ModuleTemplate.Persistence.DependencyInjection;

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
        /// Adds the persistence dependencies.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The service collection.</returns>
        public IServiceCollection AddPersistence(IConfiguration configuration)
        {
            services.AddSharedPersistence<ModuleTemplateDbContext>(configuration);

            services.AddCommon(configuration);

            services.AddRepositories();

            return services;
        }
        
        private void AddRepositories()
        {
        }
    }

    /// <summary>
    /// The <see cref="IHealthChecksBuilder"/> extensions.
    /// </summary>
    /// <param name="healthChecksBuilder">The health checks builder.</param>
    extension(IHealthChecksBuilder healthChecksBuilder)
    {
        /// <summary>
        /// Adds the persistence health checks.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The health checks builder.</returns>
        public IHealthChecksBuilder AddPersistenceHealthChecks(IConfiguration configuration)
        {
            healthChecksBuilder.AddSharedPersistenceHealthChecks<ModuleTemplateDbContext>();

            return healthChecksBuilder;
        }
    }
}