using Microsoft.EntityFrameworkCore;
using SharedCore.Persistence.IntegrationTests.TestEntities;

namespace SharedCore.Persistence.IntegrationTests.TestServices;

public class TestDbContext : BaseDbContext<TestDbContext>
{
    public DbSet<TestEntity> TestEntities { get; set; }

    public DbSet<TestChildEntity> TestChildEntities { get; set; }

    protected override string DefaultSchema => GetDefaultSchema();

    public TestDbContext(DbContextOptions options) : base(options)
    {
    }

    public static string GetDefaultSchema() => "testing";
}