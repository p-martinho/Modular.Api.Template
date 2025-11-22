using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedCore.Domain.Entities;
using SharedCore.Persistence.Constants;

namespace SharedCore.Persistence.Configurations;

/// <summary>
/// The base owned entity configuration.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <seealso cref="BaseAuditableEntityConfiguration{TEntity}"/>
[ExcludeFromCodeCoverage]
public abstract class BaseOwnedEntityConfiguration<TEntity> : BaseAuditableEntityConfiguration<TEntity>
    where TEntity : BaseOwnedEntity
{
    /// <inheritdoc />
    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        base.Configure(builder);

        builder.Property(e => e.OwnerId).HasMaxLength(MaxLength.Guid);
    }

    /// <inheritdoc />
    protected override Type? GetDirectlyDerivedType()
    {
        return null;
    }
}