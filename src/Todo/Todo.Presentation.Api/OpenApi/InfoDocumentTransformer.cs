using System.Diagnostics.CodeAnalysis;
using System.Text;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Todo.Presentation.Api.OpenApi;

/// <summary>
/// The document info document transformer.
/// </summary>
/// <seealso cref="IOpenApiDocumentTransformer"/>
[ExcludeFromCodeCoverage]
internal class InfoDocumentTransformer : IOpenApiDocumentTransformer
{
    private readonly IApiVersionDescriptionProvider _apiVersionDescriptionProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="InfoDocumentTransformer"/> class.
    /// </summary>
    /// <param name="apiVersionDescriptionProvider"></param>
    public InfoDocumentTransformer(IApiVersionDescriptionProvider apiVersionDescriptionProvider)
    {
        _apiVersionDescriptionProvider = apiVersionDescriptionProvider;
    }

    /// <inheritdoc />
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        var apiVersionDescription = _apiVersionDescriptionProvider.ApiVersionDescriptions
            .SingleOrDefault(description => description.GroupName == context.DocumentName);

        if (apiVersionDescription is null)
        {
            return Task.CompletedTask;
        }

        document.Info.Title = $"{ApiInfoDetails.Title} V{apiVersionDescription.ApiVersion}";
        document.Info.Description = BuildDescription(apiVersionDescription);
        document.Info.Version = apiVersionDescription.ApiVersion.ToString();
        document.Info.Contact = new OpenApiContact
        {
            Name = ApiInfoDetails.Contact.Name, Email = ApiInfoDetails.Contact.Email
        };
        document.Info.License = new OpenApiLicense
        {
            Name = ApiInfoDetails.Licence.Name, Url = new Uri(ApiInfoDetails.Licence.Url)
        };

        document.Servers = [];

        return Task.CompletedTask;
    }

    private static string BuildDescription(ApiVersionDescription description)
    {
        var text = new StringBuilder(ApiInfoDetails.Description);

        if (description.IsDeprecated)
        {
            text.Append(" This API version has been deprecated.");
        }

        return text.ToString();
    }
}