using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ModuleTemplate.Presentation.Api.IntegrationTests.Fixtures;
using SharedCore.Persistence.Constants;
using Testcontainers.MsSql;

[assembly: AssemblyFixture(typeof(IntegrationTestWebAppFactory))]

namespace ModuleTemplate.Presentation.Api.IntegrationTests.Fixtures;

public sealed class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder().Build();

    public async ValueTask InitializeAsync()
    {
        await _msSqlContainer.StartAsync();
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();

        await _msSqlContainer.StopAsync();
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

        // Override/add service registration (e.g. replace a specific service by a mock). This runs after the app is built (Program.cs was already executed).
        builder.ConfigureTestServices(services =>
        {
            // Use a test specific auth handler, to avoid having to handle JWT tokens.
            services.AddAuthentication(defaultScheme: TestAuthHandler.SchemeName)
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });
        });
    }
}