using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SharedCore.Domain.Abstractions;
using SharedCore.Persistence.Constants;

namespace SharedCore.Persistence.Interceptors;

/// <summary>
/// The EF Core interceptor for soft deletable entities.
/// </summary>
/// <seealso cref="SaveChangesInterceptor"/>
[ExcludeFromCodeCoverage]
internal class SoftDeletableEntityInterceptor : SaveChangesInterceptor
{
    /// <inheritdoc />
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    /// <inheritdoc />
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void UpdateEntities(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        var entriesSoftDeletable = context.ChangeTracker.Entries<ISoftDeletableEntity>()
            .Where(e => e.State == EntityState.Deleted);

        foreach (var entry in entriesSoftDeletable)
        {
            UpdateIsDeletedProperty(entry);
        }
    }

    private static void UpdateIsDeletedProperty(EntityEntry entry)
    {
        // Set to unchanged, because if set to Modified, all the fields will be added to UPDATE
        entry.State = EntityState.Unchanged;

        entry.Property(EntityProperties.IsDeleted).CurrentValue = true;

        // Set as Unchanged the owned entities. Else, it would set properties (e.g. string properties) of the owned entity as NULL, which would fail in the database for non-nullable properties.
        foreach (var navigation in entry.Navigations)
        {
            if (navigation.Metadata.TargetEntityType.IsOwned() &&
                navigation is ReferenceEntry referenceEntry &&
                referenceEntry.TargetEntry is not null)
            {
                referenceEntry.TargetEntry.State = EntityState.Unchanged;
            }
        }
    }
}