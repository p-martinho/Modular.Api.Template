using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Builder;

namespace SharedCore.Presentation.Endpoints;

/// <summary>
/// The endpoint group.
/// </summary>
public interface IEndpointGroup
{
    /// <summary>
    /// Maps the group and the endpoints of the group into the Web application.
    /// </summary>
    /// <param name="app">The Web application.</param>
    /// <param name="apiVersionSet">The API version set.</param>
    static abstract void Map(WebApplication app, ApiVersionSet apiVersionSet);
}