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
    /// The <see cref="AppIdentityUser"/> extensions.
    /// </summary>
    /// <param name="entity">The entity.</param>
    extension(AppIdentityUser entity)
    {
        /// <summary>
        /// Converts the entity into a DTO.
        /// </summary>
        /// <returns>The DTO.</returns>
        public UserInfoDto ToDto()
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
}