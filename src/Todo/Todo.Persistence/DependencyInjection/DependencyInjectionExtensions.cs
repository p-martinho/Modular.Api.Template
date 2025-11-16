using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedCore.Persistence.DependencyInjection;
using Todo.Common.DependencyInjection;
using Todo.Persistence.Repositories.TodoLists;

namespace Todo.Persistence.DependencyInjection;

/// <summary>
/// The dependency injection extensions.
/// </summary>
[ExcludeFromCodeCoverage]
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Adds the persistence dependencies.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSharedPersistence<TodoDbContext>(configuration);

        services.AddCommon(configuration);

        services.AddRepositories();

        return services;
    }

    /// <summary>
    /// Adds the persistence health checks.
    /// </summary>
    /// <param name="healthChecksBuilder">The health checks builder.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The health checks builder.</returns>
    public static IHealthChecksBuilder AddPersistenceHealthChecks(this IHealthChecksBuilder healthChecksBuilder,
        IConfiguration configuration)
    {
        healthChecksBuilder.AddSharedPersistenceHealthChecks<TodoDbContext>();

        return healthChecksBuilder;
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ITodoListRepository, TodoListRepository>();
    }
}