using System.Linq.Expressions;
using SharedCore.Domain.Abstractions;
using SharedCore.Domain.Entities;
using SharedCore.Persistence.Repositories.Models;

namespace SharedCore.Persistence.Repositories;

/// <summary>
/// The query repository. It has read operations only.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
public interface IQueryRepository<TEntity>
    where TEntity : BaseEntity, IAggregateEntity
{
    /// <summary>
    /// Gets the entity by identifier, or <c>null</c> if no entity found, asynchronous.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="isToDisableEntityTracking">Value indicating whether the tracking of the entities should be disabled.</param>
    /// <param name="isToDisableAggregateIncludes">Value indicating whether the other entities in the aggregate should not be included in the result.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The entity found, or <c>null</c>.</returns>
    Task<TEntity?> GetByIdAsync(Guid id, bool isToDisableAggregateIncludes = false,
        bool isToDisableEntityTracking = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the first entity for the given filters, or <c>null</c> if no entity found, asynchronous.
    /// </summary>
    /// <param name="filters">The filter expression.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The entity found, or <c>null</c>.</returns>
    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> filters,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the first entity for the given filters, or <c>null</c> if no entity found, asynchronous.
    /// </summary>
    /// <param name="querySpecification">The query specification.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The entity found, or <c>null</c>.</returns>
    Task<TEntity?> FirstOrDefaultAsync(QuerySpecification<TEntity> querySpecification,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the only entity for the given filters, or <c>null</c> if no entity found, asynchronous; this method throws an exception if there is more than one element in the sequence.
    /// </summary>
    /// <param name="filters">The filter expression.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>the only entity found, or <c>null</c>.</returns>
    /// <exception cref="InvalidOperationException">Source contains more than one element for the given filters.</exception>
    Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> filters,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the only entity for the given filters, or <c>null</c> if no entity found, asynchronous; this method throws an exception if there is more than one element in the sequence.
    /// </summary>
    /// <param name="querySpecification">The query specification.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>the only entity found, or <c>null</c>.</returns>
    /// <exception cref="InvalidOperationException">Source contains more than one element for the given filters.</exception>
    Task<TEntity?> SingleOrDefaultAsync(QuerySpecification<TEntity> querySpecification,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the collection of entities for the given filters asynchronous.
    /// </summary>
    /// <param name="filters">The filter expression.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The collection of entities.</returns>
    Task<IEnumerable<TEntity>> ListAsync(Expression<Func<TEntity, bool>> filters,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the collection of entities for the given query specification asynchronous.
    /// </summary>
    /// <param name="querySpecification">The query specification.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The collection of entities.</returns>
    Task<IEnumerable<TEntity>> ListAsync(QuerySpecification<TEntity> querySpecification,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the collection of entities for the given paginated query specification asynchronous.
    /// </summary>
    /// <param name="paginatedQuerySpecification">The paginated query specification.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The collection of entities.</returns>
    Task<PaginatedQueryResult<TEntity>> ListPaginatedAsync(
        PaginatedQuerySpecification<TEntity> paginatedQuerySpecification,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts the number of entities asynchronous.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of entities.</returns>
    Task<int> CountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts the number of entities for the given filters asynchronous.
    /// </summary>
    /// <param name="filters">The filter expression.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of entities.</returns>
    Task<int> CountAsync(Expression<Func<TEntity, bool>> filters, CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines if the source contains any entity asynchronous.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><c>true</c> if the source contains any entity; otherwise, <c>false</c>.</returns>
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines if the source contains any entity for the given filters asynchronous.
    /// </summary>
    /// <param name="filters">The filter expression.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><c>true</c> if the source contains any entity; otherwise, <c>false</c>.</returns>
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filters, CancellationToken cancellationToken = default);
}