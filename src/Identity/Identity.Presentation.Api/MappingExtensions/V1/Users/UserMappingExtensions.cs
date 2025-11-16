using System.Diagnostics.CodeAnalysis;
using Identity.Application.Dtos.Users;
using Identity.Application.Dtos.Users.Create;
using Identity.Application.Dtos.Users.Update;
using Identity.Presentation.Api.Dtos.V1.Users;
using Identity.Presentation.Api.Dtos.V1.Users.Create;
using Identity.Presentation.Api.Dtos.V1.Users.Update;

namespace Identity.Presentation.Api.MappingExtensions.V1.Users;

/// <summary>
/// The user mapping extensions.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class UserMappingExtensions
{
    /// <summary>
    /// Converts the application DTO into an API DTO.
    /// </summary>
    /// <param name="dto">The application DTO.</param>
    /// <returns>The API DTO.</returns>
    public static UserInfoApiDto ToApiDto(this UserInfoDto dto)
    {
        return new UserInfoApiDto
        {
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            FullName = dto.FullName
        };
    }

    /// <summary>
    /// Converts the API DTO into an application DTO.
    /// </summary>
    /// <param name="apiDto">The API DTO.</param>
    /// <returns>The application DTO.</returns>
    public static CreateUserDto ToDto(this CreateUserApiDto apiDto)
    {
        return new CreateUserDto
        {
            Email = apiDto.Email,
            Password = apiDto.Password,
            FirstName = apiDto.FirstName,
            LastName = apiDto.LastName
        };
    }

    /// <summary>
    /// Converts the API DTO into an application DTO.
    /// </summary>
    /// <param name="apiDto">The API DTO.</param>
    /// <param name="userId">The user identifier.</param>
    /// <returns>The application DTO.</returns>
    public static UpdateUserInfoDto ToDto(this UpdateUserInfoApiDto apiDto, string userId)
    {
        return new UpdateUserInfoDto
        {
            Id = userId,
            Email = apiDto.Email,
            FirstName = apiDto.FirstName,
            LastName = apiDto.LastName
        };
    }

    /// <summary>
    /// Converts the API DTO into an application DTO.
    /// </summary>
    /// <param name="apiDto">The API DTO.</param>
    /// <param name="userId">The user identifier.</param>
    /// <returns>The application DTO.</returns>
    public static UpdateUserPasswordDto ToDto(this UpdateUserPasswordApiDto apiDto, string userId)
    {
        return new UpdateUserPasswordDto
        {
            Id = userId,
            OldPassword = apiDto.OldPassword,
            NewPassword = apiDto.NewPassword
        };
    }
}