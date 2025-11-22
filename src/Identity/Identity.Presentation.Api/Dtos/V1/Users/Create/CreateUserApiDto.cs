using System.Diagnostics.CodeAnalysis;

namespace Identity.Presentation.Api.Dtos.V1.Users.Create;

/// <summary>
/// The create user API DTO.
/// </summary>
[ExcludeFromCodeCoverage]
public record CreateUserApiDto
{
    /// <summary>
    /// The user's email address which acts as a username.
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    /// The user's password.
    /// </summary>
    public required string Password { get; init; }

    /// <summary>
    /// The user's first name.
    /// </summary>
    public string? FirstName { get; init; }

    /// <summary>
    /// The user's last name.
    /// </summary>
    public string? LastName { get; init; }
}