using System.Diagnostics.CodeAnalysis;

namespace Identity.Presentation.Api.Dtos.V1.Users.Update;

/// <summary>
/// The update user info API DTO.
/// </summary>
[ExcludeFromCodeCoverage]
public record UpdateUserInfoApiDto
{
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