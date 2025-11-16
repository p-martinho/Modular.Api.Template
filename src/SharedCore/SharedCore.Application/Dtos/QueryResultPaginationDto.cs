using System.Diagnostics.CodeAnalysis;

namespace SharedCore.Application.Dtos;

/// <summary>
/// The query result pagination DTO.
/// </summary>
[ExcludeFromCodeCoverage]
public record QueryResultPaginationDto
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
    public int TotalPages
    {
        get
        {
            if (TotalRecords < 0 || PageSize <= 0)
            {
                return 0;
            }

            return (int)Math.Ceiling((double)TotalRecords / PageSize);
        }
    }

    /// <summary>
    /// Value indicating whether the current page has a next page.
    /// </summary>
    public bool HasNextPage => PageNumber * PageSize < TotalRecords;

    /// <summary>
    /// Value indicating whether the current page has a previous page.
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;
}