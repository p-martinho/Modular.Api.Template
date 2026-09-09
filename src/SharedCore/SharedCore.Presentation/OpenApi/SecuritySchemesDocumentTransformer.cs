using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using OpenIddict.Validation.AspNetCore;

namespace SharedCore.Presentation.OpenApi;

/// <summary>
/// The security schemes document transformer.
/// </summary>
/// <seealso cref="IOpenApiDocumentTransformer"/>
[ExcludeFromCodeCoverage]
public class SecuritySchemesDocumentTransformer : IOpenApiDocumentTransformer
{
    private readonly IAuthenticationSchemeProvider _authenticationSchemeProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="SecuritySchemesDocumentTransformer"/> class.
    /// </summary>
    /// <param name="authenticationSchemeProvider">The authentication scheme provider.</param>
    public SecuritySchemesDocumentTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider)
    {
        _authenticationSchemeProvider = authenticationSchemeProvider;
    }

    /// <inheritdoc />
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        var authenticationSchemes = await _authenticationSchemeProvider.GetAllSchemesAsync();

        if (authenticationSchemes.Any(authScheme =>
                authScheme.Name == OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme))
        {
            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

            document.Components.SecuritySchemes.Add(
                SharedApiInfoDetails.SecurityScheme,
                new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = SharedApiInfoDetails.SecurityScheme,
                    BearerFormat = "Json Web Token",
                    In = ParameterLocation.Header
                }
            );
        }
    }
}