using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;
using SharedCore.Application.Common.Models;

namespace Identity.Application.MappingExtensions.Users;

/// <summary>
/// The identity error extensions.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class IdentityErrorMappingExtensions
{
    /// <summary>
    /// Converts the identity error into a result detail.
    /// </summary>
    /// <param name="error">The identity error.</param>
    /// <returns>The result detail.</returns>
    public static ResultDetail ToResultDetail(this IdentityError error)
    {
        return new ResultDetail(error.Code, error.Description);
    }
}