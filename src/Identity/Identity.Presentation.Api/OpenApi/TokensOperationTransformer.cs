using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;
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

        operation.RequestBody ??= new OpenApiRequestBody { Content = new Dictionary<string, OpenApiMediaType>() };

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

        var schema = mediaType.Schema as OpenApiSchema;

        if (schema is null)
        {
            // Not possible to set properties
            return Task.CompletedTask;
        }

        schema.Properties ??= new Dictionary<string, IOpenApiSchema>();
        schema.Required ??= new HashSet<string>();

        foreach (var property in GetRequiredFormProperties())
        {
            schema.Properties[property.Key] = property.Value;
            schema.Required.Add(property.Key);
        }

        foreach (var property in GetOptionalFormProperties())
        {
            schema.Properties[property.Key] = property.Value;
        }

        return Task.CompletedTask;
    }

    private static Dictionary<string, IOpenApiSchema> GetRequiredFormProperties()
    {
        return new Dictionary<string, IOpenApiSchema>
        {
            ["grant_type"] =
                new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Description = "The grant type.",
                    Examples = new List<JsonNode>
                    {
                        JsonValue.Create("password"), JsonValue.Create("refresh_token")
                    }
                },
            ["client_id"] =
                new OpenApiSchema { Type = JsonSchemaType.String, Description = "The client identifier." },
            ["client_secret"] =
                new OpenApiSchema { Type = JsonSchemaType.String, Description = "The client secret." }
        };
    }

    private static Dictionary<string, IOpenApiSchema> GetOptionalFormProperties()
    {
        return new Dictionary<string, IOpenApiSchema>
        {
            ["username"] =
                new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Description = "The username (required in the password grant, to create a new access token)."
                },
            ["password"] =
                new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Description =
                        "The user password (required in the password grant, to create a new access token)."
                },
            ["scope"] =
                new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Description = "The scope (required in the password grant, to create a new access token).",
                    Example = JsonValue.Create("offline_access")
                },
            ["refresh_token"] =
                new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Description =
                        "The refresh token (required in the refresh_token grant, to create a new access token)."
                }
        };
    }
}