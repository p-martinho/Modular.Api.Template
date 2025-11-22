using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedCore.Persistence.Configurations;
using SharedCore.Persistence.IntegrationTests.TestEntities;

namespace SharedCore.Persistence.IntegrationTests.TestConfigurations;

internal class TestEntityConfiguration : BaseOwnedEntityConfiguration<TestEntity>
{
    public static string GetTableName() => "TestEntitiesTable";

    public override void Configure(EntityTypeBuilder<TestEntity> builder)
    {
        base.Configure(builder);

        builder.ToTable(GetTableName());

        builder.Property(e => e.Code)
            .HasMaxLength(64);

        builder.HasIndex(e => e.Code).IsUnique();

        builder.OwnsOne(e => e.OwnedEntity, o =>
        {
            o.Property(e => e.Description).HasMaxLength(512);
        });
    }
}