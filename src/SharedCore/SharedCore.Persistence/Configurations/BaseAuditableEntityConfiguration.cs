using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedCore.Domain.Entities;
using SharedCore.Persistence.Constants;

namespace SharedCore.Persistence.Configurations;

/// <summary>
/// The base auditable entity configuration.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <seealso cref="BaseEntityConfiguration{TEntity}"/>
/// <remarks>
/// For an entity of type <see cref="BaseOwnedEntity"/>, use the <see cref="BaseOwnedEntityConfiguration{TEntity}"/> instead.
/// </remarks>
[ExcludeFromCodeCoverage]
public abstract class BaseAuditableEntityConfiguration<TEntity> : BaseEntityConfiguration<TEntity>
    where TEntity : BaseAuditableEntity
{
    /// <inheritdoc />
    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        base.Configure(builder);

        builder.Property(e => e.CreatedBy).HasMaxLength(MaxLength.Guid);
        builder.Property(e => e.UpdatedBy).HasMaxLength(MaxLength.Guid);
    }

    /// <inheritdoc />
    protected override Type? GetDirectlyDerivedType()
    {
        return typeof(BaseOwnedEntity);
    }
}