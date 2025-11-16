using System.Diagnostics.CodeAnalysis;
using Identity.Common.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedCore.Persistence.DependencyInjection;

namespace Identity.Persistence.DependencyInjection;

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
        services.AddSharedPersistence<IdentityDbContext>(configuration);

        services.AddCommon(configuration);

        services.AddOpenIddictCore();

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
        healthChecksBuilder.AddSharedPersistenceHealthChecks<IdentityDbContext>();

        return healthChecksBuilder;
    }

    /// <summary>
    /// Adds the persistence for identity.
    /// </summary>
    /// <param name="identityBuilder">The identity builder.</param>
    /// <returns>The identity builder.</returns>
    public static IdentityBuilder AddIdentityStore(this IdentityBuilder identityBuilder)
    {
        identityBuilder.AddEntityFrameworkStores<IdentityDbContext>();

        return identityBuilder;
    }

    /// <summary>
    /// Adds the persistence for OpenIddict.
    /// </summary>
    /// <param name="builder">The OpenIddict builder.</param>
    /// <returns>The OpenIddict builder.</returns>
    public static OpenIddictCoreBuilder AddOpenIddictStore(this OpenIddictCoreBuilder builder)
    {
        // Register the OpenIddict core components.
        builder.UseEntityFrameworkCore()
            .UseDbContext<IdentityDbContext>();

        return builder;
    }

    private static void AddOpenIddictCore(this IServiceCollection services)
    {
        services.AddOpenIddict()
            // Register the OpenIddict core components.
            .AddCore(options =>
            {
                // Configure OpenIddict to use the Entity Framework Core stores and models.
                options.UseEntityFrameworkCore()
                    .UseDbContext<IdentityDbContext>();
            });
    }
}