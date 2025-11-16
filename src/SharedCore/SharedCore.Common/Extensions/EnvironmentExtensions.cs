using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Hosting;

namespace SharedCore.Common.Extensions;

/// <summary>
/// The environment extensions.
/// </summary>
[ExcludeFromCodeCoverage]
public static class EnvironmentExtensions
{
    private const string MigrationEnvironmentName = "Migration";

    /// <summary>
    /// Checks if the current host environment name is <c>"MigrationEnvironmentName"</c>.
    /// </summary>
    /// <param name="hostEnvironment">An instance of <see cref="IHostEnvironment"/>.</param>
    /// <returns><see langword="true"/> if the environment name is <c>"MigrationEnvironmentName"</c>, otherwise <see langword="false"/>.</returns>
    public static bool IsMigration(this IHostEnvironment hostEnvironment)
    {
        return string.Equals(hostEnvironment.EnvironmentName, MigrationEnvironmentName, StringComparison.OrdinalIgnoreCase);
    }
}