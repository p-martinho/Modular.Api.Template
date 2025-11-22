using System.Diagnostics.CodeAnalysis;

namespace Identity.Application.Dtos.Users.Update;

/// <summary>
/// The update user password DTO.
/// </summary>
[ExcludeFromCodeCoverage]
public record UpdateUserPasswordDto
{
    /// <summary>
    /// The identifier of user.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// The user's old password.
    /// </summary>
    public required string OldPassword { get; init; }

    /// <summary>
    /// The user's new password.
    /// </summary>
    public required string NewPassword { get; init; }
}