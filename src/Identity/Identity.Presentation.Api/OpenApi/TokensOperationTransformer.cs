using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Identity.Presentation.Api.OpenApi;

/// <summary>
/// The tokens operation transformer.
/// </summary>
/// <seealso cref="IOpenApiOperationTransformer"/>
[ExcludeFromCodeCoverage]
public class TokensOperationTransformer : IOpenApiOperationTransformer
{
    private const string TokensEndpoint = "connect/token";
    private const string FormContentType = "application/x-www-form-urlencoded";

    /// <inheritdoc />
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        if (!string.Equals(context.Description.RelativePath, TokensEndpoint))
        {
            return Task.CompletedTask;
        }

        operation.RequestBody ??= new OpenApiRequestBody {Content = new Dictionary<string, OpenApiMediaType>()};


        if (operation.RequestBody.Content is null)
        {
            if (operation.RequestBody is OpenApiRequestBody requestBody)
            {
                requestBody.Content = new Dictionary<string, OpenApiMediaType>();
            }
            else
            {
                // Not possible to set content
                return Task.CompletedTask;
            }
        }

        if (!operation.RequestBody.Content!.TryGetValue(FormContentType, out var mediaType))
        {
            mediaType = new OpenApiMediaType
            {
                Schema = new OpenApiSchema
                {
                    Type = JsonSchemaType.Object, Properties = new Dictionary<string, IOpenApiSchema>()
                }
            };

            operation.RequestBody.Content.Add(FormContentType, mediaType);
        }

        var formParameters = GetFormParameters();

        var schema = mediaType.Schema as OpenApiSchema;
        schema?.Required = formParameters.Select(p => p.Name!).ToHashSet();

        // Add the form parameters to the schema
        foreach (var parameter in formParameters)
        {
            schema?.Properties?[parameter.Name!] = parameter.Schema!;
        }

        return Task.CompletedTask;
    }

    private static OpenApiParameter[] GetFormParameters()
    {
        return
        [
            new OpenApiParameter
            {
                Name = "grant_type",
                Schema = new OpenApiSchema {Type = JsonSchemaType.String, Description = "The grant type."}
            },
            new OpenApiParameter
            {
                Name = "client_id",
                Schema = new OpenApiSchema {Type = JsonSchemaType.String, Description = "The client identifier."}
            },
            new OpenApiParameter
            {
                Name = "client_secret",
                Schema = new OpenApiSchema {Type = JsonSchemaType.String, Description = "The client secret."}
            }
        ];
    }
}