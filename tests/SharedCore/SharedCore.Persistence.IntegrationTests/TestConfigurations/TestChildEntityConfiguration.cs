using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedCore.Persistence.Configurations;
using SharedCore.Persistence.IntegrationTests.TestEntities;

namespace SharedCore.Persistence.IntegrationTests.TestConfigurations;

internal class TestChildEntityConfiguration : BaseAuditableEntityConfiguration<TestChildEntity>
{
    public static string GetTableName() => "TestChildEntitiesTable";

    public override void Configure(EntityTypeBuilder<TestChildEntity> builder)
    {
        base.Configure(builder);

        builder.ToTable(GetTableName());

        builder.Property(e => e.Code)
            .HasMaxLength(64);

        builder.HasIndex(e => e.Code).IsUnique();
    }
}