using System.Net.Http.Headers;

namespace Todo.Presentation.Api.IntegrationTests.Fixtures;

public class BaseIntegrationTest : IAsyncDisposable
{
    protected readonly HttpClient Client;

    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        Client = factory.CreateClient();
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: TestAuthHandler.SchemeName,
            TestAuthHandler.DefaultUserToken);
    }

    public ValueTask DisposeAsync()
    {
        Client.Dispose();

        return default;
    }
}