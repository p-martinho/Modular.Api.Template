using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharedCore.Domain.Abstractions;
using SharedCore.Domain.Entities;
using SharedCore.Persistence.Repositories.Settings;

namespace SharedCore.Persistence.Repositories;

/// <summary>
/// The repository.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <seealso cref="QueryRepository{TEntity}"/>
/// <seealso cref="IRepository{TEntity}"/>
public abstract class Repository<TEntity> : QueryRepository<TEntity>, IRepository<TEntity>
    where TEntity : BaseEntity, IAggregateEntity
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Repository{TEntity}"/> class.
    /// </summary>
    /// <param name="context">The DB context.</param>
    /// <param name="queryParametersOptions">The query parameters options.</param>
    protected Repository(DbContext context,
        IOptionsSnapshot<QueryParametersSettings> queryParametersOptions)
        : base(context, queryParametersOptions)
    {
    }

    /// <inheritdoc />
    public Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        return AppDbContext.Set<TEntity>().AddAsync(entity, cancellationToken).AsTask();
    }

    /// <inheritdoc />
    public Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        return AppDbContext.Set<TEntity>().AddRangeAsync(entities, cancellationToken);
    }

    /// <inheritdoc />
    public Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        AppDbContext.Set<TEntity>().Remove(entity);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        AppDbContext.Set<TEntity>().RemoveRange(entities);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return AppDbContext.SaveChangesAsync(cancellationToken);
    }
}