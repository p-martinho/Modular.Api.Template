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
            services.AddSharedPersistence<IdentityDbContext>(configuration);

            services.AddCommon(configuration);

            services.AddOpenIddictCore();

            return services;
        }

        private void AddOpenIddictCore()
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
            healthChecksBuilder.AddSharedPersistenceHealthChecks<IdentityDbContext>();

            return healthChecksBuilder;
        }
    }

    /// <summary>
    /// The <see cref="IdentityBuilder"/> extensions.
    /// </summary>
    /// <param name="identityBuilder">The identity builder.</param>
    extension(IdentityBuilder identityBuilder)
    {
        /// <summary>
        /// Adds the persistence for identity.
        /// </summary>
        /// <returns>The identity builder.</returns>
        public IdentityBuilder AddIdentityStore()
        {
            identityBuilder.AddEntityFrameworkStores<IdentityDbContext>();

            return identityBuilder;
        }
    }

    /// <summary>
    /// The <see cref="OpenIddictCoreBuilder"/> extensions.
    /// </summary>
    /// <param name="builder">The OpenIddict builder.</param>
    extension(OpenIddictCoreBuilder builder)
    {
        /// <summary>
        /// Adds the persistence for OpenIddict.
        /// </summary>
        /// <returns>The OpenIddict builder.</returns>
        public OpenIddictCoreBuilder AddOpenIddictStore()
        {
            // Register the OpenIddict core components.
            builder.UseEntityFrameworkCore()
                .UseDbContext<IdentityDbContext>();

            return builder;
        }
    }
}