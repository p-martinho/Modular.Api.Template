using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using SharedCore.Domain.Abstractions;
using SharedCore.Domain.Entities;

namespace SharedCore.Persistence.Repositories.Models;

/// <summary>
/// The query specification.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
[ExcludeFromCodeCoverage]
public class QuerySpecification<TEntity>
    where TEntity : BaseEntity, IAggregateEntity
{
    /// <summary>
    /// The filter expression.
    /// </summary>
    public Expression<Func<TEntity, bool>>? Filters { get; init; }

    /// <summary>
    /// The order by expression.
    /// </summary>
    public Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? OrderBy { get; init; }

    /// <summary>
    /// Value indicating whether the other entities in the aggregate should not be included in the result.
    /// </summary>
    public bool IsToDisableAggregateIncludes { get; init; }

    /// <summary>
    /// Value indicating whether the tracking of the entities should be disabled.
    /// </summary>
    public bool IsToDisableEntityTracking { get; init; }
}