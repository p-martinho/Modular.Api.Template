using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedCore.Persistence.Repositories.Settings;

namespace SharedCore.Persistence.DependencyInjection;

/// <summary>
/// The repository dependency injection extensions.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class RepositoryDependencyInjectionExtensions
{
    /// <summary>
    /// The <see cref="IServiceCollection"/> extensions.
    /// </summary>
    /// <param name="services">The service collection.</param>
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Adds the shared repositories dependencies.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The service collection.</returns>
        public IServiceCollection AddSharedRepositories(IConfiguration configuration)
        {
            services.AddSettings(configuration);

            return services;
        }

        private void AddSettings(IConfiguration configuration)
        {
            services.Configure<QueryParametersSettings>(configuration.GetSection(nameof(QueryParametersSettings)));
        }
    }
}