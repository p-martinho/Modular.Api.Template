using System.Diagnostics.CodeAnalysis;

namespace SharedCore.Presentation.Dtos;

/// <summary>
/// The result of the paginated query API DTO.
/// </summary>
[ExcludeFromCodeCoverage]
public record PaginatedQueryResultApiDto<T> where T : class
{
    /// <summary>
    /// The collection of records.
    /// </summary>
    public IEnumerable<T> Records { get; init; } = [];

    /// <summary>
    /// The query pagination.
    /// </summary>
    public QueryResultPaginationApiDto Pagination { get; init; } = new();
}