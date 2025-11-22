using System.Diagnostics.CodeAnalysis;

namespace SharedCore.Persistence.Repositories.Models;

/// <summary>
/// The query result pagination.
/// </summary>
[ExcludeFromCodeCoverage]
public record QueryResultPagination
{
    /// <summary>
    /// The page number.
    /// </summary>
    public int PageNumber { get; init; }

    /// <summary>
    /// The page size.
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// The number of records of this page.
    /// </summary>
    public int PageRecords { get; init; }

    /// <summary>
    /// The total number of records (all records, not just the records of this page).
    /// </summary>
    public int TotalRecords { get; init; }
}