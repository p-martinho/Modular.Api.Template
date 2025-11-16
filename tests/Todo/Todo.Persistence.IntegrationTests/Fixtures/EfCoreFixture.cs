using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharedCore.Persistence.Constants;
using SharedCore.Persistence.DependencyInjection;
using Testcontainers.MsSql;
using Todo.Persistence.IntegrationTests.Fixtures;

[assembly: AssemblyFixture(typeof(EfCoreFixture))]

namespace Todo.Persistence.IntegrationTests.Fixtures;

public class EfCoreFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder().Build();
    private IServiceScope? _serviceScope;

    internal TodoDbContext Context => _serviceScope!.ServiceProvider.GetRequiredService<TodoDbContext>();

    public virtual async ValueTask InitializeAsync()
    {
        await _msSqlContainer.StartAsync();

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { $"ConnectionStrings:{ConnectionStrings.SqlDefault}", _msSqlContainer.GetConnectionString() }
            })
            .Build();

        var services = new ServiceCollection();

        // IHostEnvironment is used to decide whether it should apply migrations.
        services.AddScoped<IHostEnvironment, TestHostEnvironment>();

        // Adding using DI, for integration testing, to include interceptors, migration, etc.
        services.AddSharedPersistence<TodoDbContext>(configuration);

        var serviceProvider = services.BuildServiceProvider();

        _serviceScope = serviceProvider.CreateScope();
    }

    public virtual async ValueTask DisposeAsync()
    {
        _serviceScope?.Dispose();
        await _msSqlContainer.DisposeAsync();
    }
}