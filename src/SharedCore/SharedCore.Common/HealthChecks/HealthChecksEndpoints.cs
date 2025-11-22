using System.Diagnostics.CodeAnalysis;

namespace SharedCore.Common.HealthChecks;

/// <summary>
/// The health checks endpoint paths.
/// </summary>
[ExcludeFromCodeCoverage]
public static class HealthChecksEndpoints
{
    /// <summary>
    /// The endpoint path for the aliveness health checks.
    /// </summary>
    public const string AlivenessEndpointPath = "/alive";

    /// <summary>
    /// The endpoint path for the standard health checks.
    /// </summary>
    public const string HealthEndpointPath = "/health";

    /// <summary>
    /// The endpoint path for the full health checks.
    /// </summary>
    public const string FullHealthEndpointPath = "/health/full";
}