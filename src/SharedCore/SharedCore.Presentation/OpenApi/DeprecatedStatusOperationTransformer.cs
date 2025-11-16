using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace SharedCore.Presentation.OpenApi;

/// <summary>
/// The deprecated status operation transformer.
/// </summary>
/// <seealso cref="IOpenApiOperationTransformer"/>
[ExcludeFromCodeCoverage]
public class DeprecatedStatusOperationTransformer : IOpenApiOperationTransformer
{
    /// <inheritdoc />
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        operation.Deprecated |= context.Description.IsDeprecated();

        return Task.CompletedTask;
    }
}