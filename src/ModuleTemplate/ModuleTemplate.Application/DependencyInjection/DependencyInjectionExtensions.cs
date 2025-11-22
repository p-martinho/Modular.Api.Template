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
    /// The <see cref="IServiceCollection"/> extensions.
    /// </summary>
    /// <param name="services">The service collection.</param>
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Adds the application dependencies.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The service collection.</returns>
        public IServiceCollection AddApplication(IConfiguration configuration)
        {
            services.AddSharedApplication(configuration);

            services.AddPersistence(configuration);

            services.AddValidators();

            services.AddCommandHandlers();

            services.AddQueryHandlers();

            return services;
        }

        private void AddValidators()
        {
        }

        private void AddCommandHandlers()
        {
        }

        private void AddQueryHandlers()
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
        /// Adds the application health checks.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The health checks builder.</returns>
        public IHealthChecksBuilder AddApplicationHealthChecks(IConfiguration configuration)
        {
            healthChecksBuilder.AddPersistenceHealthChecks(configuration);

            return healthChecksBuilder;
        }
    }
}