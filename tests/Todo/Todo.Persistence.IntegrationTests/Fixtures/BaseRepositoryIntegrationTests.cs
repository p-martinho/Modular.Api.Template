using Microsoft.Extensions.DependencyInjection;

namespace Todo.Persistence.IntegrationTests.Fixtures;

public abstract class BaseRepositoryIntegrationTests : IClassFixture<EfCoreFixture>, IDisposable
{
    private readonly IServiceScope _testScope;

    internal readonly TodoDbContext DbContext;

    protected BaseRepositoryIntegrationTests(EfCoreFixture fixture)
    {
        _testScope = fixture.ServiceProvider.CreateScope();

        DbContext = _testScope.ServiceProvider.GetRequiredService<TodoDbContext>();
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