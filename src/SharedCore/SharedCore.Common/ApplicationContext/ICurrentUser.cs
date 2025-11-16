namespace SharedCore.Common.ApplicationContext;

/// <summary>
/// The current user.
/// </summary>
public interface ICurrentUser
{
    /// <summary>
    /// Value indicating whether a user is authenticated.
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// The user identifier.
    /// </summary>
    string? UserId { get; }

    /// <summary>
    /// Value indicating whether the user is in a specific role.
    /// </summary>
    /// <param name="roleName">The role name.</param>
    /// <returns><c>true</c> if the user has the role; <c>false</c> otherwise.</returns>
    bool IsInRole(string roleName);

    /// <summary>
    /// Value indicating whether the user has a specific claim, with specific value.
    /// </summary>
    /// <param name="claimType">The claim type.</param>
    /// <param name="claimValue">The claim value.</param>
    /// <returns><c>true</c> if user has the claim; <c>false</c> otherwise.</returns>
    bool HasClaim(string claimType, string claimValue);
}