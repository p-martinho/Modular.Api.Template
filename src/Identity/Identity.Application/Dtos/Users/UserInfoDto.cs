using System.Diagnostics.CodeAnalysis;

namespace Identity.Application.Dtos.Users;

/// <summary>
/// The user info DTO.
/// </summary>
[ExcludeFromCodeCoverage]
public record UserInfoDto
{
    /// <summary>
    /// The identifier.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// The user's email address which acts as a username.
    /// </summary>
    public required string Email { get; init; }

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