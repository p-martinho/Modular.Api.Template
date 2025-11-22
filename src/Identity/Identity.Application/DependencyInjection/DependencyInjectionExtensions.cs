using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Identity.Application.Commands.OpenId.Seed;
using Identity.Application.Commands.Roles.Seed;
using Identity.Application.Commands.Tokens.Exchange;
using Identity.Application.Commands.Users.Create;
using Identity.Application.Commands.Users.Update;
using Identity.Application.Dtos.Users.Create;
using Identity.Application.Dtos.Users.Update;
using Identity.Application.Queries.Users.GetById;
using Identity.Domain.Entities.Users;
using Identity.Persistence.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Quartz;
using SharedCore.Application.DependencyInjection;
using SharedCore.Common.Extensions;

namespace Identity.Application.DependencyInjection;

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
        /// <param name="hostEnvironment">The host environment.</param>
        /// <returns>The service collection.</returns>
        public IServiceCollection AddApplication(IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            services.AddSharedApplication(configuration);

            services.AddPersistence(configuration);

            services.AddIdentity();

            services.AddOpenIddictServer(configuration, hostEnvironment);

            services.AddValidators();

            services.AddCommandHandlers();

            services.AddQueryHandlers();

            return services;
        }

        private void AddIdentity()
        {
            services.AddIdentityCore<AppIdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddSignInManager()
                .AddDefaultTokenProviders()
                .AddIdentityStore();
        }

        private void AddOpenIddictServer(IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            // OpenIddict offers native integration with Quartz.NET to perform scheduled tasks
            // (like pruning orphaned authorizations/tokens from the database) at regular intervals.
            services.AddQuartz(options =>
            {
                options.UseSimpleTypeLoader();
                options.UseInMemoryStore();
            });

            // Register the Quartz.NET service and configure it to block shutdown until jobs are complete.
            services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

            services.AddOpenIddict()
                // Register the OpenIddict core components.
                .AddCore(options =>
                {
                    // Enable Quartz.NET integration.
                    options.UseQuartz();

                    // Store is configured in the Persistence layer.
                    options.AddOpenIddictStore();
                })
                // Register the OpenIddict server components.
                .AddServer(options =>
                {
                    // Enable the flows.
                    options.AllowPasswordFlow()
                        .AllowRefreshTokenFlow();

                    // Enable the endpoints.
                    options.SetTokenEndpointUris("connect/token");

                    // Register the signing and encryption credentials.
                    if (hostEnvironment.IsDevelopment() || hostEnvironment.IsMigration())
                    {
                        options.AddDevelopmentEncryptionCertificate()
                            .DisableAccessTokenEncryption();

                        options.AddDevelopmentSigningCertificate();
                    }
                    else
                    {
                        options.AddEncryptionKey(new SymmetricSecurityKey(
                            Convert.FromBase64String(configuration["IdentitySettings:EncryptionKey"] ?? string.Empty)));

                        options.AddSigningCertificate(configuration["IdentitySettings:SigningCertificateThumbprint"] ??
                                                      string.Empty);
                    }

                    var customIssuer = configuration["IdentitySettings:Issuer"];

                    // Require only when we want to override it (e.g. in local docker compose).
                    if (customIssuer is not null)
                    {
                        options.SetIssuer(customIssuer);
                    }

                    // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
                    var aspNetOptions = options.UseAspNetCore()
                        .EnableTokenEndpointPassthrough();

                    // Disable HTTPS requirement (e.g. useful in local docker compose).
                    if (hostEnvironment.IsDevelopment() &&
                        configuration.GetSection("IdentitySettings:DisableHttps").Get<bool>())
                    {
                        aspNetOptions.DisableTransportSecurityRequirement();
                    }
                })
                // Register the OpenIddict validation components.
                .AddValidation(options =>
                {
                    // Import the configuration from the local OpenIddict server instance.
                    options.UseLocalServer();

                    options.AddAudiences(configuration["IdentitySettings:Audience"] ?? string.Empty);

                    // Register the ASP.NET Core host.
                    options.UseAspNetCore();
                });
        }

        private void AddValidators()
        {
            services.AddScoped<IValidator<CreateUserDto>, CreateUserValidator>();
            services.AddScoped<IValidator<UpdateUserInfoDto>, UpdateUserInfoValidator>();
            services.AddScoped<IValidator<UpdateUserPasswordDto>, UpdateUserPasswordValidator>();
        }

        private void AddCommandHandlers()
        {
            services.AddScoped<ISeedOpenIdTestingResourcesCommandHandler, SeedOpenIdTestingResourcesCommandHandler>();
            services.AddScoped<ISeedRolesCommandHandler, SeedRolesCommandHandler>();

            services.AddScoped<IExchangeTokenCommandHandler, ExchangeTokenCommandHandler>();

            services.AddScoped<ICreateUserCommandHandler, CreateUserCommandHandler>();
            services.AddScoped<IUpdateUserInfoCommandHandler, UpdateUserInfoCommandHandler>();
            services.AddScoped<IUpdateUserPasswordCommandHandler, UpdateUserPasswordCommandHandler>();
        }

        private void AddQueryHandlers()
        {
            services.AddScoped<IGetUserByIdQueryHandler, GetUserByIdQueryHandler>();
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