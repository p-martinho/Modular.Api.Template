using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SharedCore.Presentation.Endpoints;

namespace SharedCore.Presentation.Extensions;

/// <summary>
/// The extensions related with endpoints.
/// </summary>
[ExcludeFromCodeCoverage]
public static class EndpointExtensions
{
    private const string EndpointGroupPrefix = "api";

    /// <summary>
    /// The <see cref="WebApplication"/> extensions.
    /// </summary>
    /// <param name="app">The Web application.</param>
    extension(WebApplication app)
    {
        /// <summary>
        /// Maps the API endpoints into the Web application.
        /// </summary>
        /// <typeparam name="TProgram">A type from the API assembly with the endpoints to map.</typeparam>
        /// <param name="deprecatedApiVersions">The deprecated API versions.</param>
        /// <returns>The Web application.</returns>
        public WebApplication MapEndpoints<TProgram>(IEnumerable<ApiVersion>? deprecatedApiVersions = null)
        {
            var apiBuilder = app.NewVersionedApi();

            foreach (var version in deprecatedApiVersions ?? [])
            {
                apiBuilder.HasDeprecatedApiVersion(version);
            }

            ScanAndMapEndpointGroups(typeof(TProgram).Assembly, apiBuilder);

            return app;
        }
    }

    /// <summary>
    /// The <see cref="IVersionedEndpointRouteBuilder"/> extensions.
    /// </summary>
    /// <param name="apiBuilder">The endpoint builder.</param>
    extension(IVersionedEndpointRouteBuilder apiBuilder)
    {
        /// <summary>
        /// Maps an endpoint group into the endpoint builder.
        /// </summary>
        /// <param name="groupName">The name for the group.</param>
        /// <param name="apiVersion">The API version for the group.</param>
        /// <param name="isAuthorizationRequired">Value indicating whether the group should require authorization.</param>
        /// /// <param name="isDeprecated">Value indicating whether the group is deprecated.</param>
        /// <returns>The route group builder.</returns>
        public RouteGroupBuilder MapEndpointGroup(string groupName, ApiVersion? apiVersion = null,
            bool isAuthorizationRequired = false, bool isDeprecated = false)
        {
            var group = apiBuilder.MapGroup($"{EndpointGroupPrefix}/{groupName}")
                .HasApiVersion(apiVersion ?? new ApiVersion(1, 0))
                .WithTags(groupName);

            if (isAuthorizationRequired)
            {
                group.RequireAuthorization();
            }

            if (isDeprecated)
            {
                group.AsDeprecated();
            }

            return group;
        }
    }

    /// <summary>
    /// The <see cref="IEndpointConventionBuilder"/> extensions.
    /// </summary>
    /// <param name="endpointBuilder">The endpoint builder.</param>
    extension(IEndpointConventionBuilder endpointBuilder)
    {
        /// <summary>
        /// Sets all the endpoints in the endpoint builder as deprecated in the OpenAPI documents.
        /// </summary>
        /// <param name="description">The description to set in the OpenAPI operations.</param>
        /// <returns>The endpoint builder.</returns>
        public IEndpointConventionBuilder AsDeprecated(string? description = null)
        {
            endpointBuilder.AddOpenApiOperationTransformer((operation, _, _) =>
            {
                operation.Deprecated = true;

                if (description is not null)
                {
                    operation.Description = description;
                }

                return Task.CompletedTask;
            });

            return endpointBuilder;
        }
    }

    private static void ScanAndMapEndpointGroups(Assembly assemblyToScan, IVersionedEndpointRouteBuilder apiBuilder)
    {
        var endpointGroupTypes = assemblyToScan.GetTypes()
            .Where(type => typeof(IEndpointGroup).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract);

        foreach (var endpointGroupType in endpointGroupTypes)
        {
            // Get the static Map method.
            var mapMethod = endpointGroupType.GetMethod(nameof(IEndpointGroup.Map),
                BindingFlags.Static | BindingFlags.Public);

            // Invoke the Map method.
            mapMethod?.Invoke(null, [apiBuilder]);
        }
    }
}