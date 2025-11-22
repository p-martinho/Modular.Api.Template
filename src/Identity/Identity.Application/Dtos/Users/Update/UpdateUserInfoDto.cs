using System.Diagnostics.CodeAnalysis;

namespace Identity.Application.Dtos.Users.Update;

/// <summary>
/// The update user info DTO.
/// </summary>
[ExcludeFromCodeCoverage]
public record UpdateUserInfoDto
{
    /// <summary>
    /// The identifier of user.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// The user's email address which acts as a username.
    /// </summary>
    public string? Email { get; init; }

    /// <summary>
    /// The user's first name.
    /// </summary>
    public string? FirstName { get; init; }

    /// <summary>
    /// The user's last name.
    /// </summary>
    public string? LastName { get; init; }
}