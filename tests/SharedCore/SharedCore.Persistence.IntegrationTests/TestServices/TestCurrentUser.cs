using SharedCore.Common.ApplicationContext;

namespace SharedCore.Persistence.IntegrationTests.TestServices;

public class TestCurrentUser : ICurrentUser
{
    private string? _currentRole;

    public bool IsAuthenticated => true;

    public string UserId { get; set; } = "UserId";

    public bool IsInRole(string roleName) => _currentRole == roleName;

    public bool HasClaim(string claimType, string claimValue) => false;

    public void SetCurrentUserRole(string? roleName) => _currentRole = roleName;
}