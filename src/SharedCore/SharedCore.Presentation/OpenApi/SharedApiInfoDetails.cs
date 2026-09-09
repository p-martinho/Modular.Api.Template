using System.Diagnostics.CodeAnalysis;

namespace SharedCore.Presentation.OpenApi;

/// <summary>
/// The shared information about the APIs.
/// </summary>
[ExcludeFromCodeCoverage]
public static class SharedApiInfoDetails
{
    /// <summary>
    /// The security scheme used by the APIs.
    /// </summary>
    public const string SecurityScheme = "Bearer";
}