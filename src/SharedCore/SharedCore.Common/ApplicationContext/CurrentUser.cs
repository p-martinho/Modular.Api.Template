using Microsoft.AspNetCore.Http;
using SharedCore.Common.Authorization;

namespace SharedCore.Common.ApplicationContext;

/// <summary>
/// The current user.
/// </summary>
/// <seealso cref="ICurrentUser"/>
internal class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor? _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="CurrentUser"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">The HTTP context accessor.</param>
    public CurrentUser(IHttpContextAccessor? httpContextAccessor = null)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <inheritdoc />
    public bool IsAuthenticated => _httpContextAccessor?.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    /// <inheritdoc />
    public string? UserId => _httpContextAccessor?.HttpContext?.User?.FindFirst(JwtClaims.Subject)?.Value;

    /// <inheritdoc />
    public bool IsInRole(string roleName) => _httpContextAccessor?.HttpContext?.User?.IsInRole(roleName) ?? false;

    /// <inheritdoc />
    public bool HasClaim(string claimType, string claimValue) =>
        _httpContextAccessor?.HttpContext?.User?.HasClaim(claimType, claimValue) ?? false;
}