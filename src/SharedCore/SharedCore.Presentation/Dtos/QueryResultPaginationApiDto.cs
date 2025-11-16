using System.Diagnostics.CodeAnalysis;

namespace SharedCore.Presentation.Dtos;

/// <summary>
/// The query result pagination API DTO.
/// </summary>
[ExcludeFromCodeCoverage]
public record QueryResultPaginationApiDto
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

    /// <summary>
    /// The total number of pages.
    /// </summary>
    public int TotalPages { get; init; }

    /// <summary>
    /// Value indicating whether the current page has a next page.
    /// </summary>
    public bool HasNextPage { get; init; }

    /// <summary>
    /// Value indicating whether the current page has a previous page.
    /// </summary>
    public bool HasPreviousPage { get; init; }
}