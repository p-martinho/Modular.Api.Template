using System.Diagnostics.CodeAnalysis;
using Identity.Application.DependencyInjection;
using SharedCore.Presentation.DependencyInjection;

namespace Identity.Presentation.Api.DependencyInjection;

/// <summary>
/// The dependency injection extensions.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class DependencyInjectionExtensions
{
    /// <summary>
    /// The <see cref="IServiceCollection"/> extensions.
    /// </summary>
    /// <param name="services">The service collection.</param>
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Adds the custom health checks dependencies.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The service collection.</returns>
        public IServiceCollection AddCustomHealthChecks(IConfiguration configuration)
        {
            // Add here specific health checks for this API. Default health checks were already registered in ServiceDefaults project.

            services.AddHealthChecks()
                .AddApplicationHealthChecks(configuration);

            return services;
        }

        /// <summary>
        /// Adds the API dependencies.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="hostEnvironment">The host environment.</param>
        /// <returns>The service collection.</returns>
        public IServiceCollection AddApiDependencies(IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            // Configuration of OpenIdDict validation is in the Application layer, along with the Server configuration.
            services.AddSharedPresentation(configuration, hostEnvironment, isToConfigOpenIdDictValidation: false);

            services.AddApplication(configuration, hostEnvironment);

            return services;
        }
    }
}