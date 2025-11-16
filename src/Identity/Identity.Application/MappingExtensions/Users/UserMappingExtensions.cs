using System.Diagnostics.CodeAnalysis;
using Identity.Application.Dtos.Users;
using Identity.Domain.Entities.Users;

namespace Identity.Application.MappingExtensions.Users;

/// <summary>
/// The user mapping extensions.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class UserMappingExtensions
{
    /// <summary>
    /// Converts the entity into a DTO.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <returns>The DTO.</returns>
    public static UserInfoDto ToDto(this AppIdentityUser entity)
    {
        return new UserInfoDto
        {
            Id = entity.Id,
            Email = entity.Email!,
            FirstName = entity.Name.FirstName,
            LastName = entity.Name.LastName,
            FullName = entity.Name.GetFullName()
        };
    }
}