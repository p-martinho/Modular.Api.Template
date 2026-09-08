using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharedCore.Common.ApplicationContext;
using SharedCore.Persistence.Constants;
using SharedCore.Persistence.DependencyInjection;
using SharedCore.Persistence.IntegrationTests.Fixtures;
using SharedCore.Persistence.IntegrationTests.TestServices;
using Testcontainers.MsSql;

[assembly: AssemblyFixture(typeof(EfCoreFixture))]

namespace SharedCore.Persistence.IntegrationTests.Fixtures;

public class EfCoreFixture : IAsyncLifetime
{
    private const string MsSqlImageName = "mcr.microsoft.com/mssql/server:2022-latest";

    private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder(MsSqlImageName).Build();

    public IServiceProvider ServiceProvider = null!;

    public virtual async ValueTask InitializeAsync()
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

        // One of the EF Core interceptors uses ICurrentUser to set audit properties.
        services.AddScoped<ICurrentUser, TestCurrentUser>();

        // Adding using DI, for integration testing, to include interceptors, migration, etc.
        services.AddSharedPersistence<TestDbContext>(configuration);

        ServiceProvider = services.BuildServiceProvider();
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);

        GC.SuppressFinalize(this);
    }

    protected virtual ValueTask DisposeAsyncCore()
    {
        return _msSqlContainer.DisposeAsync();
    }
}