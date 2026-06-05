using Asp.Versioning.Builder;

namespace SharedCore.Presentation.Endpoints;

/// <summary>
/// The endpoint group.
/// </summary>
public interface IEndpointGroup
{
    /// <summary>
    /// Maps the group and the endpoints of the group into the Web application.
    /// </summary>
    /// <param name="apiBuilder">The API endpoint builder.</param>
    static abstract void Map(IVersionedEndpointRouteBuilder apiBuilder);
}