using System.Diagnostics.CodeAnalysis;
using SharedCore.Application.Dtos;
using SharedCore.Persistence.Repositories.Models;

namespace SharedCore.Application.MappingExtensions;

/// <summary>
/// The query result pagination mapping extensions.
/// </summary>
[ExcludeFromCodeCoverage]
public static class QueryResultPaginationMappingExtensions
{
    /// <summary>
    /// Converts the query result pagination into a DTO.
    /// </summary>
    /// <param name="queryResultPagination">The query result pagination.</param>
    /// <returns>The DTO.</returns>
    public static QueryResultPaginationDto ToDto(this QueryResultPagination queryResultPagination)
    {
        return new QueryResultPaginationDto
        {
            PageNumber = queryResultPagination.PageNumber,
            PageSize = queryResultPagination.PageSize,
            PageRecords = queryResultPagination.PageRecords,
            TotalRecords = queryResultPagination.TotalRecords
        };
    }
}