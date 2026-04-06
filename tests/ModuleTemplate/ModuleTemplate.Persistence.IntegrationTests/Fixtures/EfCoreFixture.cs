using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModuleTemplate.Persistence.IntegrationTests.Fixtures;
using SharedCore.Persistence.Constants;
using SharedCore.Persistence.DependencyInjection;
using Testcontainers.MsSql;

[assembly: AssemblyFixture(typeof(EfCoreFixture))]

namespace ModuleTemplate.Persistence.IntegrationTests.Fixtures;

public sealed class EfCoreFixture : IAsyncLifetime
{
    private const string MsSqlImageName = "mcr.microsoft.com/mssql/server:2022-CU24-ubuntu-22.04";

    private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder(MsSqlImageName).Build();
    private IServiceScope? _serviceScope;

    internal ModuleTemplateDbContext Context =>
        _serviceScope!.ServiceProvider.GetRequiredService<ModuleTemplateDbContext>();

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
        services.AddSharedPersistence<ModuleTemplateDbContext>(configuration);

        var serviceProvider = services.BuildServiceProvider();

        _serviceScope = serviceProvider.CreateScope();
    }

    public async ValueTask DisposeAsync()
    {
        await _msSqlContainer.DisposeAsync();

        _serviceScope?.Dispose();
    }
}