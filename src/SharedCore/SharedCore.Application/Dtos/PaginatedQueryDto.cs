using System.Diagnostics.CodeAnalysis;

namespace SharedCore.Application.Dtos;

/// <summary>
/// The paginated query DTO.
/// </summary>
[ExcludeFromCodeCoverage]
public record PaginatedQueryDto
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
    /// The name of the property to order by.
    /// </summary>
    public string? OrderBy { get; init; }

    /// <summary>
    /// Value indicating whether the sort order should be descending, instead of ascending.
    /// </summary>
    public bool IsDescendingOrder { get; init; }

    /// <summary>
    /// The name of the property to filter by.
    /// </summary>
    public string? FilterBy { get; init; }

    /// <summary>
    /// The value of the property to filter by.
    /// </summary>
    public string? FilterValue { get; init; }
}