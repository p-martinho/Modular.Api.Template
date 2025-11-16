using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedCore.Common.DependencyInjection;

namespace ModuleTemplate.Common.DependencyInjection;

/// <summary>
/// The dependency injection extensions.
/// </summary>
[ExcludeFromCodeCoverage]
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Adds the common dependencies.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddCommon(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSharedCommon(configuration);

        return services;
    }
}