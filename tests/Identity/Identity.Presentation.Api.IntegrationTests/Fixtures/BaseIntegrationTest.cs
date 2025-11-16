using Identity.Domain.Entities.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Presentation.Api.IntegrationTests.Fixtures;

public class BaseIntegrationTest : IAsyncDisposable
{
    private readonly IServiceScope _testScope;

    protected readonly HttpClient Client;
    protected readonly UserManager<AppIdentityUser> UserManager;

    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _testScope = factory.Services.CreateScope();

        Client = factory.CreateClient();
        Client.BaseAddress = new Uri("https://localhost"); // Set to HTTPS

        UserManager = _testScope.ServiceProvider.GetRequiredService<UserManager<AppIdentityUser>>();
    }

    public ValueTask DisposeAsync()
    {
        UserManager.Dispose();
        _testScope.Dispose();

        return default;
    }
}