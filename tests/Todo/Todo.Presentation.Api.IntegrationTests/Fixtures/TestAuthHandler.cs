using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedCore.Common.Authorization;

namespace Todo.Presentation.Api.IntegrationTests.Fixtures;

internal class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private const string AuthorizationHeaderName = "Authorization";

    public const string SchemeName = "TestScheme";
    public const string DefaultUserId = "DefaultUserId";
    public const string OtherUserId = "OtherUserId";
    public const string AdminUserId = "AdminUserId";
    public const string DefaultUserToken = "DefaultUserToken";
    public const string OtherUserToken = "OtherUserToken";
    public const string AdminToken = "AdminToken";

    public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(AuthorizationHeaderName, out var values))
        {
            return Task.FromResult(AuthenticateResult.Fail("Missing Authorization Header"));
        }

        var token = values.ToString();

        var isDefaultUser = token is $"{SchemeName} {DefaultUserToken}";
        var isOtherUser = token is $"{SchemeName} {OtherUserToken}";
        var isAdminUser = token is $"{SchemeName} {AdminToken}";

        if (!isDefaultUser && !isAdminUser && !isOtherUser)
        {
            return Task.FromResult(AuthenticateResult.Fail("Unauthorized"));
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, "Test user"),
            new(JwtClaims.Subject, GetUserId(isDefaultUser, isAdminUser))
        };

        if (isAdminUser)
        {
            claims.Add(new Claim(ClaimTypes.Role, UserRoles.Admin));
        }

        var identity = new ClaimsIdentity(claims, "Test");

        var principal = new ClaimsPrincipal(identity);

        var ticket = new AuthenticationTicket(principal, SchemeName);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    private static string GetUserId(bool isDefaultUser, bool isAdminUser)
    {
        if (isDefaultUser)
        {
            return DefaultUserId;
        }

        return isAdminUser ? AdminUserId : OtherUserId;
    }
}