using Microsoft.Extensions.DependencyInjection;
using SharedCore.Common.ApplicationContext;
using SharedCore.Persistence.IntegrationTests.TestServices;

namespace SharedCore.Persistence.IntegrationTests.Fixtures;

public class RepositoryFixture : EfCoreFixture
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