using System.Diagnostics.CodeAnalysis;
using Asp.Versioning;
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
    /// Adds OpenAPI documents to the services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="apiVersions">The API versions to add.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddOpenApiDocuments(this IServiceCollection services,
        IEnumerable<ApiVersion> apiVersions)
    {
        foreach (var version in apiVersions)
        {
            services.AddOpenApi(GetDocumentName(version), options =>
            {
                options.AddDocumentTransformer<InfoDocumentTransformer>();
                options.AddDocumentTransformer<SecuritySchemesDocumentTransformer>();
                options.AddOperationTransformer<AuthorizationOperationTransformer>();
                options.AddOperationTransformer<ApiVersionOperationTransformer>();
                options.AddOperationTransformer<DeprecatedStatusOperationTransformer>();
            });
        }

        return services;
    }

    /// <summary>
    /// Register the Scalar UI into the application.
    /// </summary>
    /// <param name="app">The Web application.</param>
    /// <param name="apiVersions">The API versions to add.</param>
    /// <returns>The Web application.</returns>
    public static WebApplication MapScalar(this WebApplication app, IEnumerable<ApiVersion> apiVersions)
    {
        var documentNames = apiVersions.Select(GetDocumentName).ToArray();

        app.MapScalarApiReference(options =>
        {
            options.WithTitle(ApiInfoDetails.Title);

            options.WithPreferredScheme(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)
                .WithHttpBearerAuthentication(bearerOptions =>
                {
                    bearerOptions.Token = "your-token";
                });

            if (documentNames.Length != 0)
            {
                options.AddDocuments(documentNames);
            }
        });

        return app;
    }

    private static string GetDocumentName(ApiVersion apiVersion)
    {
        return $"v{apiVersion}";
    }
}