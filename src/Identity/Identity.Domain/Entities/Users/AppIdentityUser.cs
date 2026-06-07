using Identity.Domain.ValueObjects.Users;
using Microsoft.AspNetCore.Identity;

namespace Identity.Domain.Entities.Users;

/// <summary>
/// The application identity user.
/// </summary>
/// <seealso cref="IdentityUser"/>
public class AppIdentityUser : IdentityUser
{
    private AppIdentityUser()
    {
    }

    /// <summary>
    /// The name.
    /// </summary>
    public AppIdentityUserName Name { get; private set; } = new(null, null);

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="email">The email.</param>
    /// <param name="firstName">The first name.</param>
    /// /// <param name="lastName">The last name.</param>
    /// <returns>The created user; or <c>null</c> in case of validation failure.</returns>
    public static AppIdentityUser? Create(string email, string? firstName, string? lastName)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return null;
        }

        return new AppIdentityUser
        {
            Email = email,
            Name = new AppIdentityUserName(firstName, lastName)
        };
    }

    /// <summary>
    /// Updates the name of the user.
    /// </summary>
    /// <param name="firstName">The first name.</param>
    /// <param name="lastName">The last name.</param>
    /// <returns></returns>
    public bool UpdateName(string? firstName, string? lastName)
    {
        Name = new AppIdentityUserName(firstName, lastName);

        return true;
    }
}