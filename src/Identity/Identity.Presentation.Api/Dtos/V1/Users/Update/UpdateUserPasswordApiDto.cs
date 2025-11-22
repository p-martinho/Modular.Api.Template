using System.Diagnostics.CodeAnalysis;

namespace Identity.Presentation.Api.Dtos.V1.Users.Update;

/// <summary>
/// The update user password API DTO.
/// </summary>
[ExcludeFromCodeCoverage]
public record UpdateUserPasswordApiDto
{
    /// <summary>
    /// The user's old password.
    /// </summary>
    public required string OldPassword { get; init; }

    /// <summary>
    /// The user's new password.
    /// </summary>
    public required string NewPassword { get; init; }
}