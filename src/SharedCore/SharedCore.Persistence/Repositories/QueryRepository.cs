using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Options;
using SharedCore.Domain.Abstractions;
using SharedCore.Domain.Entities;
using SharedCore.Persistence.Repositories.Models;
using SharedCore.Persistence.Repositories.Settings;

namespace SharedCore.Persistence.Repositories;

/// <summary>
/// The query repository.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <seealso cref="IQueryRepository{TEntity}"/>
public abstract class QueryRepository<TEntity> : IQueryRepository<TEntity>
    where TEntity : BaseEntity, IAggregateEntity
{
    private const int DefaultMaxLimit = 100;

    private readonly QueryParametersSettings _queryParametersSettings;

    /// <summary>
    /// The EF Core DB context.
    /// </summary>
    protected readonly DbContext AppDbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryRepository{TEntity}"/> class.
    /// </summary>
    /// <param name="context">The DB context.</param>
    /// <param name="queryParametersOptions">The query parameters options.</param>
    protected QueryRepository(DbContext context,
        IOptionsSnapshot<QueryParametersSettings> queryParametersOptions)
    {
        AppDbContext = context;
        _queryParametersSettings = queryParametersOptions.Value;
    }

    /// <inheritdoc />
    public Task<TEntity?> GetByIdAsync(Guid id, bool isToDisableAggregateIncludes = false,
        bool isToDisableEntityTracking = false, CancellationToken cancellationToken = default)
    {
        var querySpecification = new QuerySpecification<TEntity>
        {
            Filters = e => e.Id == id,
            IsToDisableAggregateIncludes = isToDisableAggregateIncludes,
            IsToDisableEntityTracking = isToDisableEntityTracking
        };

        return SingleOrDefaultAsync(querySpecification, cancellationToken);
    }

    /// <inheritdoc />
    public Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> filters,
        CancellationToken cancellationToken = default)
    {
        var querySpecification = new QuerySpecification<TEntity>
        {
            Filters = filters,
            IsToDisableAggregateIncludes = false,
            IsToDisableEntityTracking = false
        };

        return FirstOrDefaultAsync(querySpecification, cancellationToken);
    }

    /// <inheritdoc />
    public Task<TEntity?> FirstOrDefaultAsync(QuerySpecification<TEntity> querySpecification,
        CancellationToken cancellationToken = default)
    {
        var query = GetQueryableFromQuerySpecification(querySpecification);

        return query.FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> filters,
        CancellationToken cancellationToken = default)
    {
        var querySpecification = new QuerySpecification<TEntity>
        {
            Filters = filters,
            IsToDisableAggregateIncludes = false,
            IsToDisableEntityTracking = false
        };

        return SingleOrDefaultAsync(querySpecification, cancellationToken);
    }

    /// <inheritdoc />
    public Task<TEntity?> SingleOrDefaultAsync(QuerySpecification<TEntity> querySpecification,
        CancellationToken cancellationToken = default)
    {
        var query = GetQueryableFromQuerySpecification(querySpecification);

        return query.SingleOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<IEnumerable<TEntity>> ListAsync(Expression<Func<TEntity, bool>> filters,
        CancellationToken cancellationToken = default)
    {
        var querySpecification = new QuerySpecification<TEntity>
        {
            Filters = filters,
            IsToDisableAggregateIncludes = false,
            IsToDisableEntityTracking = false
        };

        return ListAsync(querySpecification, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TEntity>> ListAsync(QuerySpecification<TEntity> querySpecification,
        CancellationToken cancellationToken = default)
    {
        var query = GetQueryableFromQuerySpecification(querySpecification);

        query = query.Take(GetMaxLimit());

        return await query.ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PaginatedQueryResult<TEntity>> ListPaginatedAsync(
        PaginatedQuerySpecification<TEntity> paginatedQuerySpecification, CancellationToken cancellationToken = default)
    {
        var query = GetQueryableFromQuerySpecification(paginatedQuerySpecification);

        var pageNumber = GetPageNumber(paginatedQuerySpecification.PageNumber);

        var pageSize = GetPageSize(paginatedQuerySpecification.PageSize);

        var offset = GetOffset(pageNumber, pageSize);

        var totalRecords = await query.CountAsync(cancellationToken);

        var queryPaginated = query.Skip(offset).Take(pageSize);

        var records = await queryPaginated.ToListAsync(cancellationToken);

        return new PaginatedQueryResult<TEntity>
        {
            Records = records,
            Pagination = new QueryResultPagination
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                PageRecords = records.Count,
                TotalRecords = totalRecords
            }
        };
    }

    /// <inheritdoc />
    public Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        var query = GetQueryable(isToDisableAggregateIncludes: true);

        return query.CountAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<int> CountAsync(Expression<Func<TEntity, bool>> filters, CancellationToken cancellationToken = default)
    {
        var query = GetQueryable(isToDisableAggregateIncludes: true).Where(filters);

        return query.CountAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        var query = GetQueryable(isToDisableAggregateIncludes: true);

        return query.AnyAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filters, CancellationToken cancellationToken = default)
    {
        var query = GetQueryable(isToDisableAggregateIncludes: true).Where(filters);

        return query.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// Gets the function with the navigation properties to include, the ones that belongs to the aggregate.
    /// </summary>
    /// <returns>The function with the navigation properties to include, if any; otherwise, <c>null</c>.</returns>
    /// <remarks>The default is <c>null</c> (does not include related entities).</remarks>
    protected virtual Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? GetDefaultAggregateIncludes()
    {
        return null;
    }

    /// <summary>
    /// Gets the default permissions filter.
    /// </summary>
    /// <returns>The default permissions filter, if any; otherwise, <c>null</c>.</returns>
    /// <remarks>The default is <c>null</c> (no permissions filter).</remarks>
    protected virtual Expression<Func<TEntity, bool>>? GetDefaultPermissionsFilter()
    {
        return null;
    }

    private IQueryable<TEntity> GetQueryable(bool isToDisableAggregateIncludes)
    {
        var query = AppDbContext.Set<TEntity>().AsQueryable();

        var includes = isToDisableAggregateIncludes
            ? null
            : GetDefaultAggregateIncludes();

        if (includes is not null)
        {
            query = includes(query);
        }

        var defaultPermissionsFilter = GetDefaultPermissionsFilter();

        if (defaultPermissionsFilter is not null)
        {
            query = query.Where(defaultPermissionsFilter);
        }

        return query;
    }

    private IQueryable<TEntity> GetQueryableFromQuerySpecification(QuerySpecification<TEntity> querySpecification)
    {
        var query = GetQueryable(querySpecification.IsToDisableAggregateIncludes);

        if (querySpecification.IsToDisableEntityTracking)
        {
            query = query.AsNoTracking();
        }

        if (querySpecification.Filters is not null)
        {
            query = query.Where(querySpecification.Filters);
        }

        query = querySpecification.OrderBy is not null ? querySpecification.OrderBy(query) : query.OrderBy(e => e.Id);

        return query;
    }

    private int GetMaxLimit()
    {
        return _queryParametersSettings.MaxLimit > 0 ? _queryParametersSettings.MaxLimit : DefaultMaxLimit;
    }

    private static int GetPageNumber(int queryPageNumber)
    {
        return queryPageNumber > 0 ? queryPageNumber : 1;
    }

    private int GetPageSize(int queryPageSize)
    {
        var maxLimit = GetMaxLimit();

        if (queryPageSize <= 0 || queryPageSize > maxLimit)
        {
            return maxLimit;
        }

        return queryPageSize;
    }

    private static int GetOffset(int pageNumber, int pageSize)
    {
        return (pageNumber - 1) * pageSize;
    }
}