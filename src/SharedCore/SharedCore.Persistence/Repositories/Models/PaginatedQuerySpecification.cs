using System.Diagnostics.CodeAnalysis;
using SharedCore.Domain.Abstractions;
using SharedCore.Domain.Entities;

namespace SharedCore.Persistence.Repositories.Models;

/// <summary>
/// The paginated query specification.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <seealso cref="QuerySpecification{TEntity}"/>
[ExcludeFromCodeCoverage]
public class PaginatedQuerySpecification<TEntity> : QuerySpecification<TEntity>
    where TEntity : BaseEntity, IAggregateEntity
{
    /// <summary>
    /// The page number.
    /// </summary>
    public int PageNumber { get; init; }

    /// <summary>
    /// The page size.
    /// </summary>
    public int PageSize { get; init; }
}