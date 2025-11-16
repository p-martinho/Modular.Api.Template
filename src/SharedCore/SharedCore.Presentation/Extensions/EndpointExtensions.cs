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
    /// Maps the API endpoints into the Web application.
    /// </summary>
    /// <typeparam name="TProgram">A type from the API assembly with the endpoints to map.</typeparam>
    /// <param name="app">The Web application.</param>
    /// <param name="activeApiVersions">The active API versions.</param>
    /// <param name="deprecatedApiVersions">The deprecated API versions.</param>
    /// <returns>The Web application.</returns>
    public static WebApplication MapEndpoints<TProgram>(this WebApplication app,
        IEnumerable<ApiVersion> activeApiVersions,
        IEnumerable<ApiVersion>? deprecatedApiVersions = null)
    {
        var apiVersionSet = app.BuildApiVersionSet(activeApiVersions, deprecatedApiVersions ?? []);

        app.ScanAndMapEndpointGroups(typeof(TProgram).Assembly, apiVersionSet);

        return app;
    }

    /// <summary>
    /// Maps an endpoint group into the Web application.
    /// </summary>
    /// <param name="app">The Web application.</param>
    /// <param name="groupName">The name for the group.</param>
    /// <param name="apiVersionSet">The API version set.</param>
    /// <param name="apiVersion">The API version for the group.</param>
    /// <param name="isAuthorizationRequired">Value indicating whether the group should require authorization.</param>
    /// <returns>The Web application.</returns>
    public static RouteGroupBuilder MapEndpointGroup(this WebApplication app, string groupName,
        ApiVersionSet apiVersionSet, ApiVersion? apiVersion = null, bool isAuthorizationRequired = false)
    {
        var group = app.MapGroup($"{EndpointGroupPrefix}/{groupName}")
            .WithApiVersionSet(apiVersionSet)
            .MapToApiVersion(apiVersion ?? new ApiVersion(1, 0))
            .WithTags(groupName);

        if (isAuthorizationRequired)
        {
            group.RequireAuthorization();
        }

        return group;
    }

    private static void ScanAndMapEndpointGroups(this WebApplication app, Assembly assemblyToScan,
        ApiVersionSet apiVersionSet)
    {
        var endpointGroupTypes = assemblyToScan.GetTypes()
            .Where(type => typeof(IEndpointGroup).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract);

        foreach (var endpointGroupType in endpointGroupTypes)
        {
            // Get the static Map method.
            var mapMethod = endpointGroupType.GetMethod(nameof(IEndpointGroup.Map),
                BindingFlags.Static | BindingFlags.Public);

            // Invoke the Map method.
            mapMethod?.Invoke(null, [app, apiVersionSet]);
        }
    }

    private static ApiVersionSet BuildApiVersionSet(this WebApplication app, IEnumerable<ApiVersion> activeApiVersions,
        IEnumerable<ApiVersion> deprecatedApiVersions)
    {
        var apiVersionSet = app.NewApiVersionSet();

        foreach (var version in activeApiVersions)
        {
            apiVersionSet.HasApiVersion(version);
        }

        foreach (var version in deprecatedApiVersions)
        {
            apiVersionSet.HasDeprecatedApiVersion(version);
        }

        apiVersionSet.ReportApiVersions();

        return apiVersionSet.Build();
    }
}