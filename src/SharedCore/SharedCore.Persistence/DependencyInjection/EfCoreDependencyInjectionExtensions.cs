using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharedCore.Common.HealthChecks;
using SharedCore.Persistence.Constants;
using SharedCore.Persistence.Interceptors;

namespace SharedCore.Persistence.DependencyInjection;

/// <summary>
/// The EF Core dependency injection extensions.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class EfCoreDependencyInjectionExtensions
{
    /// <summary>
    /// Adds the shared EF Core dependencies.
    /// </summary>
    /// <typeparam name="TContext">The specific type of DB context.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="isToAddDefaultDatabaseProvider">Value indicating whether it should add the default database provider. If set to false, configure the context by overriding the OnConfiguring(DbContextOptionsBuilder) method in your derived context.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddSharedEfCore<TContext>(this IServiceCollection services,
        IConfiguration configuration, bool isToAddDefaultDatabaseProvider = true)
        where TContext : DbContext
    {
        services.AddInterceptors();

        services.AddDbContext<TContext>((serviceProvider, optionsBuilder) =>
        {
            if (isToAddDefaultDatabaseProvider)
            {
                optionsBuilder.AddDefaultDatabaseProvider(configuration);
            }

            optionsBuilder.AddInterceptors(GetDefaultInterceptors(serviceProvider));
        });

        ApplyDatabaseMigrationsIfDevelopment<TContext>(services);

        return services;
    }

    /// <summary>
    /// Adds the shared EF Core health checks.
    /// </summary>
    /// <param name="healthChecksBuilder">The health checks builder.</param>
    public static IHealthChecksBuilder AddSharedEfCoreHealthChecks<TContext>(
        this IHealthChecksBuilder healthChecksBuilder)
        where TContext : DbContext
    {
        healthChecksBuilder.AddDbContextCheck<TContext>(tags: [HealthChecksTags.DbContext]);

        return healthChecksBuilder;
    }

    private static void AddInterceptors(this IServiceCollection services)
    {
        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, SoftDeletableEntityInterceptor>();
    }

    private static IEnumerable<IInterceptor> GetDefaultInterceptors(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetServices<ISaveChangesInterceptor>();
    }

    private static void AddDefaultDatabaseProvider(this DbContextOptionsBuilder optionsBuilder,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(ConnectionStrings.SqlDefault);

        optionsBuilder
            .UseSqlServer(connectionString,
                sqlOptionsBuilder =>
                    sqlOptionsBuilder.EnableRetryOnFailure()
                        .UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery));
    }

    private static void ApplyDatabaseMigrationsIfDevelopment<TContext>(IServiceCollection services)
        where TContext : DbContext
    {
        using var scope = services.BuildServiceProvider().CreateScope();

        var isDevelopment = scope.ServiceProvider.GetRequiredService<IHostEnvironment>().IsDevelopment();

        if (!isDevelopment)
        {
            return;
        }

        var context = scope.ServiceProvider.GetRequiredService<TContext>();

        context.Database.Migrate();
    }
}