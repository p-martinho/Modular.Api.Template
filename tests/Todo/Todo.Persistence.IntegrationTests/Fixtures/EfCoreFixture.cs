using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharedCore.Persistence.Constants;
using SharedCore.Persistence.DependencyInjection;
using Testcontainers.MsSql;
using Todo.Persistence.IntegrationTests.Fixtures;

[assembly: AssemblyFixture(typeof(EfCoreFixture))]

namespace Todo.Persistence.IntegrationTests.Fixtures;

public sealed class EfCoreFixture : IAsyncLifetime
{
    private const string MsSqlImageName = "mcr.microsoft.com/mssql/server:2022-CU24-ubuntu-22.04";

    private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder(MsSqlImageName).Build();

    public IServiceProvider ServiceProvider = null!;

    public async ValueTask InitializeAsync()
    {
        await _msSqlContainer.StartAsync(TestContext.Current.CancellationToken);

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

        ServiceProvider = services.BuildServiceProvider();
    }

    public ValueTask DisposeAsync()
    {
        return _msSqlContainer.DisposeAsync();
    }
}