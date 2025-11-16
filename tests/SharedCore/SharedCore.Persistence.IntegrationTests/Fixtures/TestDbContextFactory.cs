using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SharedCore.Persistence.IntegrationTests.TestServices;

namespace SharedCore.Persistence.IntegrationTests.Fixtures;

/// <summary>
/// The test DB context factory.
/// </summary>
/// <remarks>This class is only needed to create migrations using the command (in the project folder): <code>dotnet ef migrations add InitialMigration</code></remarks>
public class TestDbContextFactory : IDesignTimeDbContextFactory<TestDbContext>
{
    public TestDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TestDbContext>();
        optionsBuilder.UseSqlServer();

        return new TestDbContext(optionsBuilder.Options);
    }
}