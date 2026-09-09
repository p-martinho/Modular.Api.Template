namespace Identity.Domain.ValueObjects.Users;

/// <summary>
/// The name of the application identity user.
/// </summary>
/// <param name="FirstName">The first name.</param>
/// <param name="LastName">The last name.</param>
public record AppIdentityUserName(string? FirstName, string? LastName)
{
    /// <summary>
    /// Gets the user full name.
    /// </summary>
    /// <returns>The full name (or <c>null</c> if the name is empty).</returns>
    public string? GetFullName()
    {
        if (string.IsNullOrWhiteSpace(FirstName))
        {
            return string.IsNullOrWhiteSpace(LastName) ? null : $"{LastName}";
        }

        return string.IsNullOrWhiteSpace(LastName) ? $"{FirstName}" : $"{FirstName} {LastName}";
    }
}