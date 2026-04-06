using Identity.Presentation.Api.IntegrationTests.Fixtures;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using SharedCore.Persistence.Constants;
using Testcontainers.MsSql;

[assembly: AssemblyFixture(typeof(IntegrationTestWebAppFactory))]

namespace Identity.Presentation.Api.IntegrationTests.Fixtures;

public sealed class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private const string MsSqlImageName = "mcr.microsoft.com/mssql/server:2022-CU24-ubuntu-22.04";

    private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder(MsSqlImageName).Build();

    public async ValueTask InitializeAsync()
    {
        await _msSqlContainer.StartAsync(TestContext.Current.CancellationToken);
    }

    public override async ValueTask DisposeAsync()
    {
        await _msSqlContainer.DisposeAsync();

        await base.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Override the connection string for the database, with the one from the container.
        // The same could be done removing the DbContextOptions<> and call services.AddDbContext<>(...) in ConfigureTestServices(), but this way we don't change how it is being registered and configured.
        // Using Environment.SetEnvironmentVariable instead of builder.ConfigureAppConfiguration(), because we need to set it earlier, to apply the migrations.
        Environment.SetEnvironmentVariable($"ConnectionStrings:{ConnectionStrings.SqlDefault}",
            _msSqlContainer.GetConnectionString());

        // Override/add logging configuration (e.g. add a provider that writes to the test output). This runs after the app is built (Program.cs was already executed).
        builder.ConfigureLogging(loggingBuilder =>
        {
            // Clear providers to try to improve performance of the tests.
            loggingBuilder.ClearProviders();
        });
    }
}