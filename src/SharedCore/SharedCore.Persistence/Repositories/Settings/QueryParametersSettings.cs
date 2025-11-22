using System.Diagnostics.CodeAnalysis;

namespace SharedCore.Persistence.Repositories.Settings;

/// <summary>
/// The query parameters settings.
/// </summary>
[ExcludeFromCodeCoverage]
public class QueryParametersSettings
{
    /// <summary>
    /// The maximum number of records a query can retrieve.
    /// </summary>
    public int MaxLimit { get; set; } = 100;
}