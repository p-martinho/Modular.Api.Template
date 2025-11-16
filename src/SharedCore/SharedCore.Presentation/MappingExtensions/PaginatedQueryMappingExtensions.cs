using System.Diagnostics.CodeAnalysis;
using SharedCore.Application.Dtos;
using SharedCore.Presentation.Dtos;

namespace SharedCore.Presentation.MappingExtensions;

/// <summary>
/// The paginated query mapping extensions.
/// </summary>
[ExcludeFromCodeCoverage]
public static class PaginatedQueryMappingExtensions
{
    /// <summary>
    /// Converts the API DTO into an application DTO.
    /// </summary>
    /// <param name="apiDto">The API DTO.</param>
    /// <returns>The application DTO.</returns>
    public static PaginatedQueryDto ToDto(this PaginatedQueryApiDto apiDto)
    {
        return new PaginatedQueryDto
        {
            PageNumber = apiDto.PageNumber.GetValueOrDefault(),
            PageSize = apiDto.PageSize.GetValueOrDefault(),
            OrderBy = apiDto.OrderBy,
            IsDescendingOrder = apiDto.IsDescendingOrder.GetValueOrDefault(),
            FilterBy = apiDto.FilterBy,
            FilterValue = apiDto.FilterValue
        };
    }
}