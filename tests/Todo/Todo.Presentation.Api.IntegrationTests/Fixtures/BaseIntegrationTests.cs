using System.Net.Http.Headers;

namespace Todo.Presentation.Api.IntegrationTests.Fixtures;

public class BaseIntegrationTests : IAsyncDisposable
{
    protected readonly HttpClient Client;

    protected BaseIntegrationTests(IntegrationTestWebAppFactory factory)
    {
        Client = factory.CreateClient();
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: TestAuthHandler.SchemeName,
            TestAuthHandler.DefaultUserToken);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);

        GC.SuppressFinalize(this);
    }

    protected virtual ValueTask DisposeAsyncCore()
    {
        Client.Dispose();

        return default;
    }
}