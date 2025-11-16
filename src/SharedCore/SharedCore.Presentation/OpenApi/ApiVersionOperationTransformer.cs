using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace SharedCore.Presentation.OpenApi;

/// <summary>
/// The API version operation transformer.
/// </summary>
/// <seealso cref="IOpenApiOperationTransformer"/>
[ExcludeFromCodeCoverage]
public class ApiVersionOperationTransformer : IOpenApiOperationTransformer
{
    /// <inheritdoc />
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        // Find parameter named "api-version" and add a description to it
        var apiVersionParameter = operation.Parameters?.FirstOrDefault(p => p.Name == "api-version");

        if (apiVersionParameter is null)
        {
            return Task.CompletedTask;
        }

        apiVersionParameter.Description = "The API version, in the format 'major.minor'.";
        apiVersionParameter.Schema.Example = new OpenApiString(context.DocumentName.Replace("v", string.Empty));

        return Task.CompletedTask;
    }
}