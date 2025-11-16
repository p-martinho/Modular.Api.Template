using System.Diagnostics.CodeAnalysis;

namespace Identity.Presentation.Api.Dtos.V1.Users;

/// <summary>
/// The user info API DTO.
/// </summary>
[ExcludeFromCodeCoverage]
public record UserInfoApiDto
{
    /// <summary>
    /// The user's email address which acts as a username.
    /// </summary>
    public string Email { get; init; } = null!;

    /// <summary>
    /// The user's first name.
    /// </summary>
    public string? FirstName { get; init; }

    /// <summary>
    /// The user's last name.
    /// </summary>
    public string? LastName { get; init; }

    /// <summary>
    /// The user's full name.
    /// </summary>
    public string? FullName { get; init; }
}