using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SharedCore.Common.ApplicationContext;
using SharedCore.Domain.Abstractions;
using SharedCore.Domain.Entities;

namespace SharedCore.Persistence.Interceptors;

/// <summary>
/// The EF Core interceptor for auditable entities.
/// </summary>
/// <seealso cref="SaveChangesInterceptor"/>
[ExcludeFromCodeCoverage]
internal class AuditableEntityInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUser? _currentUser;

    /// <summary>
    /// Initializes an instance of the class <see cref="AuditableEntityInterceptor"/>.
    /// </summary>
    /// <param name="currentUser">The current user.</param>
    public AuditableEntityInterceptor(ICurrentUser? currentUser = null)
    {
        _currentUser = currentUser;
    }

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

    private void UpdateEntities(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        var auditableEntries = context.ChangeTracker
            .Entries<BaseAuditableEntity>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || HasChangedOwnedEntities(e) ||
                        (e.State == EntityState.Deleted && e.Entity is ISoftDeletableEntity));

        var utcNow = DateTimeOffset.UtcNow;

        foreach (var entry in auditableEntries)
        {
            UpdateEntityAuditableProperties(entry, utcNow);
        }
    }

    private static bool HasChangedOwnedEntities(EntityEntry entityEntry)
    {
        return entityEntry.References.Any(r =>
            r.TargetEntry != null &&
            r.TargetEntry.Metadata.IsOwned() &&
            r.TargetEntry.State is EntityState.Added or EntityState.Modified);
    }

    private void UpdateEntityAuditableProperties(EntityEntry<BaseAuditableEntity> entry, DateTimeOffset utcNow)
    {
        if (entry.State == EntityState.Added)
        {
            entry.Property<DateTimeOffset>(nameof(BaseAuditableEntity.CreatedAt)).CurrentValue = utcNow;
            entry.Property<string?>(nameof(BaseAuditableEntity.CreatedBy)).CurrentValue = _currentUser?.UserId;
        }

        entry.Property<DateTimeOffset>(nameof(BaseAuditableEntity.UpdatedAt)).CurrentValue = utcNow;
        entry.Property<string?>(nameof(BaseAuditableEntity.UpdatedBy)).CurrentValue = _currentUser?.UserId;
    }
}