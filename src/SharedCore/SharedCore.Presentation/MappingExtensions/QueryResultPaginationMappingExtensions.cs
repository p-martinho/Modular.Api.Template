using System.Diagnostics.CodeAnalysis;
using SharedCore.Application.Dtos;
using SharedCore.Presentation.Dtos;

namespace SharedCore.Presentation.MappingExtensions;

/// <summary>
/// The query result pagination mapping extensions.
/// </summary>
[ExcludeFromCodeCoverage]
public static class QueryResultPaginationMappingExtensions
{
    /// <summary>
    /// Converts the application DTO into an API DTO.
    /// </summary>
    /// <param name="dto">The application DTO.</param>
    /// <returns>The API DTO.</returns>
    public static QueryResultPaginationApiDto ToApiDto(this QueryResultPaginationDto dto)
    {
        return new QueryResultPaginationApiDto
        {
            PageNumber = dto.PageNumber,
            PageSize = dto.PageSize,
            PageRecords = dto.PageRecords,
            TotalRecords = dto.TotalRecords,
            TotalPages = dto.TotalPages,
            HasNextPage = dto.HasNextPage,
            HasPreviousPage = dto.HasPreviousPage
        };
    }
}