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
    /// Adds the shared repositories dependencies.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddSharedRepositories(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSettings(configuration);

        return services;
    }

    private static void AddSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<QueryParametersSettings>(configuration.GetSection(nameof(QueryParametersSettings)));
    }
}