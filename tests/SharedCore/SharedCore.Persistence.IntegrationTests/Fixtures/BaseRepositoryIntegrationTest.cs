using Microsoft.Extensions.DependencyInjection;
using SharedCore.Common.ApplicationContext;
using SharedCore.Persistence.IntegrationTests.TestServices;

namespace SharedCore.Persistence.IntegrationTests.Fixtures;

public abstract class BaseRepositoryIntegrationTest : IClassFixture<RepositoryFixture>, IDisposable
{
    private readonly IServiceScope _testScope;

    protected readonly TestDbContext DbContext;
    protected readonly TestCurrentUser CurrentUser;

    protected BaseRepositoryIntegrationTest(RepositoryFixture fixture)
    {
        _testScope = fixture.ServiceProvider.CreateScope();

        var context = _testScope.ServiceProvider.GetRequiredService<TestDbContext>();

        var currentUser = (_testScope.ServiceProvider.GetRequiredService<ICurrentUser>() as TestCurrentUser)!;

        DbContext = context;
        CurrentUser = currentUser;
    }

    public void Dispose()
    {
        DbContext.Dispose();
        _testScope.Dispose();
    }
}