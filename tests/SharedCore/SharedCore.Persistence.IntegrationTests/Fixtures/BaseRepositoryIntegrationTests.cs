using Microsoft.Extensions.DependencyInjection;
using SharedCore.Common.ApplicationContext;
using SharedCore.Persistence.IntegrationTests.TestServices;

namespace SharedCore.Persistence.IntegrationTests.Fixtures;

public abstract class BaseRepositoryIntegrationTests : IClassFixture<TestDataEfCoreFixture>, IDisposable
{
    private readonly IServiceScope _testScope;

    protected readonly TestDbContext DbContext;
    protected readonly TestCurrentUser CurrentUser;

    protected BaseRepositoryIntegrationTests(TestDataEfCoreFixture fixture)
    {
        _testScope = fixture.ServiceProvider.CreateScope();

        DbContext = _testScope.ServiceProvider.GetRequiredService<TestDbContext>();

        CurrentUser = (_testScope.ServiceProvider.GetRequiredService<ICurrentUser>() as TestCurrentUser)!;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        _testScope.Dispose();
    }
}