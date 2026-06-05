using System.Diagnostics.CodeAnalysis;
using OpenIddict.Validation.AspNetCore;
using Scalar.AspNetCore;
using SharedCore.Presentation.OpenApi;

namespace ModuleTemplate.Presentation.Api.OpenApi;

/// <summary>
/// The OpenAPI related extensions.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class OpenApiExtensions
{
    /// <summary>
    /// The <see cref="IServiceCollection"/> extensions.
    /// </summary>
    /// <param name="services">The service collection.</param>
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Adds OpenAPI documents to the services.
        /// </summary>
        /// <returns>The service collection.</returns>
        public IServiceCollection AddOpenApiDocuments()
        {
            // Call "AddOpenApi" after "AddApiVersioning" to ensure Asp.Versioning's variant is used.
            // This variant of "AddOpenApi" is required to properly integrate with API versioning and generate versioned OpenAPI documents.
            services.AddApiVersioning().AddOpenApi(options =>
            {
                options.Document.AddDocumentTransformer<InfoDocumentTransformer>();
                options.Document.AddDocumentTransformer<SecuritySchemesDocumentTransformer>();
                options.Document.AddOperationTransformer<AuthorizationOperationTransformer>();
                options.Document.AddOperationTransformer<ApiVersionOperationTransformer>();
                options.Document.AddOperationTransformer<DeprecatedStatusOperationTransformer>();
                options.Document.AddSchemaTransformer<ProblemDetailsSchemaTransformer>();
            });

            return services;
        }
    }

    /// <summary>
    /// The <see cref="WebApplication"/> extensions.
    /// </summary>
    /// <param name="app">The Web application.</param>
    extension(WebApplication app)
    {
        /// <summary>
        /// Register the Scalar UI into the application.
        /// </summary>
        /// <returns>The Web application.</returns>
        public WebApplication MapScalar()
        {
            var versions = app.DescribeApiVersions();
            var defaultVersion = versions.LastOrDefault();

            app.MapScalarApiReference(options =>
            {
                options.WithTitle(ApiInfoDetails.Title);

                options.AddPreferredSecuritySchemes(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)
                    .AddHttpAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme,
                        securityScheme =>
                        {
                            securityScheme.Token = "your-token";
                        });

                foreach (var version in versions)
                {
                    options.AddDocument(version.GroupName, isDefault: version == defaultVersion);
                }
            });

            return app;
        }
    }
}