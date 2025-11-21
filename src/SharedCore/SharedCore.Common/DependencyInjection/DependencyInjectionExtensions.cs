using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedCore.Common.ApplicationContext;

namespace SharedCore.Common.DependencyInjection;

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
        /// Adds the shared common dependencies.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The service collection.</returns>
        public IServiceCollection AddSharedCommon(IConfiguration configuration)
        {
            services.AddScoped<ICurrentUser, CurrentUser>();

            return services;
        }
    }
}