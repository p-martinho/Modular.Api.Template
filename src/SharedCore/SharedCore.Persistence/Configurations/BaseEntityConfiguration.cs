using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedCore.Domain.Entities;

namespace SharedCore.Persistence.Configurations;

/// <summary>
/// The base entity configuration.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <seealso cref="IEntityTypeConfiguration{TEntity}"/>
/// <remarks>
/// For an entity of type <see cref="BaseOwnedEntity"/>, use the <see cref="BaseOwnedEntityConfiguration{TEntity}"/> instead.
/// For an entity of type <see cref="BaseAuditableEntity"/>, use the <see cref="BaseAuditableEntityConfiguration{TEntity}"/> instead.
/// </remarks>
[ExcludeFromCodeCoverage]
public abstract class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : BaseEntity
{
    /// <inheritdoc />
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        if (!IsValidEntityTypeForThisConfiguration())
        {
            throw new InvalidOperationException(GetExceptionMessage());
        }

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).ValueGeneratedNever();
    }

    /// <summary>
    /// Gets the directly derived type, to then validate if this class is not being used directly, instead of using a more specific class.
    /// </summary>
    /// <returns>The directly derived type.</returns>
    protected virtual Type? GetDirectlyDerivedType()
    {
        return typeof(BaseAuditableEntity);
    }

    private bool IsValidEntityTypeForThisConfiguration()
    {
        var derivedType = GetDirectlyDerivedType();

        return derivedType is null || !derivedType.IsAssignableFrom(typeof(TEntity));
    }

    private string GetExceptionMessage()
    {
        var derivedType = GetDirectlyDerivedType();

        return $"For the entity '{typeof(TEntity)}', which is derived from '{derivedType}', use a more specific base entity configuration, instead of '{GetType().BaseType}'.";
    }
}