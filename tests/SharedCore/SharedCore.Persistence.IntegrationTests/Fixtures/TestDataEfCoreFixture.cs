using Microsoft.Extensions.DependencyInjection;
using SharedCore.Common.ApplicationContext;
using SharedCore.Persistence.IntegrationTests.Fixtures;
using SharedCore.Persistence.IntegrationTests.TestServices;

[assembly: AssemblyFixture(typeof(TestDataEfCoreFixture))]

namespace SharedCore.Persistence.IntegrationTests.Fixtures;

public sealed class TestDataEfCoreFixture : EfCoreFixture
{
    public override async ValueTask InitializeAsync()
    {
        await base.InitializeAsync();

        using var scope = ServiceProvider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<TestDbContext>();

        var currentUser = scope.ServiceProvider.GetRequiredService<ICurrentUser>();

        var databaseSeeder = new DatabaseSeeder(context, currentUser);

        await databaseSeeder.SeedDatabaseAsync(TestContext.Current.CancellationToken);
    }
}