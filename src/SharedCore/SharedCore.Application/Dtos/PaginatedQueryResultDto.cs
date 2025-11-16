using System.Diagnostics.CodeAnalysis;

namespace SharedCore.Application.Dtos;

/// <summary>
/// The result of the paginated query DTO.
/// </summary>
[ExcludeFromCodeCoverage]
public record PaginatedQueryResultDto<T> where T : class
{
    /// <summary>
    /// The collection of records.
    /// </summary>
    public IReadOnlyCollection<T> Records { get; init; } = [];

    /// <summary>
    /// The query pagination.
    /// </summary>
    public QueryResultPaginationDto Pagination { get; init; } = new();
}