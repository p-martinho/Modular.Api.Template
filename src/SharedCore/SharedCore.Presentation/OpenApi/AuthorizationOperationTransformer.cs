using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using OpenIddict.Validation.AspNetCore;

namespace SharedCore.Presentation.OpenApi;

/// <summary>
/// The authorization operation transformer.
/// </summary>
/// <seealso cref="IOpenApiOperationTransformer"/>
[ExcludeFromCodeCoverage]
public class AuthorizationOperationTransformer : IOpenApiOperationTransformer
{
    private readonly IAuthenticationSchemeProvider _authenticationSchemeProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizationOperationTransformer"/> class.
    /// </summary>
    /// <param name="authenticationSchemeProvider">The authentication scheme provider.</param>
    public AuthorizationOperationTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider)
    {
        _authenticationSchemeProvider = authenticationSchemeProvider;
    }

    /// <inheritdoc />
    public async Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        var metadata = context.Description.ActionDescriptor.EndpointMetadata;

        if (!metadata.OfType<IAuthorizeData>().Any())
        {
            return;
        }

        operation.Responses ??= new OpenApiResponses();

        operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
        operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });

        var authenticationSchemes = await _authenticationSchemeProvider.GetAllSchemesAsync();

        if (authenticationSchemes.Any(s => s.Name == OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme))
        {
            var securitySchemeReference =
                new OpenApiSecuritySchemeReference(SharedApiInfoDetails.SecurityScheme, context.Document);

            operation.Security = new List<OpenApiSecurityRequirement> { new() { { securitySchemeReference, [] } } };
        }
    }
}