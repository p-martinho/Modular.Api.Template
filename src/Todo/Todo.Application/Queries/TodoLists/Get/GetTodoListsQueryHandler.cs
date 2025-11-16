using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using SharedCore.Application.Dtos;
using SharedCore.Application.MappingExtensions;
using SharedCore.Application.Queries;
using SharedCore.Application.Queries.Models;
using SharedCore.Persistence.Repositories.Models;
using Todo.Application.Dtos.TodoLists;
using Todo.Application.MappingExtensions.TodoLists;
using Todo.Domain.Entities.TodoLists;
using Todo.Persistence.Repositories.TodoLists;

namespace Todo.Application.Queries.TodoLists.Get;

/// <summary>
/// The get to do lists query handler.
/// </summary>
/// <seealso cref="IGetTodoListsQueryHandler"/>
internal class GetTodoListsQueryHandler : QueryHandler<PaginatedQueryDto, PaginatedQueryResultDto<TodoListDto>>,
    IGetTodoListsQueryHandler
{
    private const string PropertyKeyForName = "name";
    private const string PropertyKeyForOwnerId = "ownerid";

    private readonly ITodoListRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetTodoListsQueryHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="repository">The to do list repository.</param>
    public GetTodoListsQueryHandler(ILogger<GetTodoListsQueryHandler> logger,
        ITodoListRepository repository)
        : base(logger)
    {
        _repository = repository;
    }

    /// <inheritdoc />
    protected override async Task<QueryOut<PaginatedQueryResultDto<TodoListDto>>> HandleQueryInAsync(
        PaginatedQueryDto queryIn, CancellationToken cancellationToken)
    {
        var querySpecification = new PaginatedQuerySpecification<TodoList>
        {
            PageNumber = queryIn.PageNumber,
            PageSize = queryIn.PageSize,
            OrderBy = GetOrderByExpression(queryIn),
            Filters = GetFiltersExpression(queryIn),
            IsToDisableEntityTracking = true
        };

        var paginatedQueryResult = await _repository.ListPaginatedAsync(querySpecification, cancellationToken);

        var result = new PaginatedQueryResultDto<TodoListDto>
        {
            Pagination = paginatedQueryResult.Pagination.ToDto(),
            Records = paginatedQueryResult.Records.Select(e => e.ToDto()).ToList().AsReadOnly()
        };

        return QueryOut<PaginatedQueryResultDto<TodoListDto>>.Success(result);
    }

    private static Func<IQueryable<TodoList>, IOrderedQueryable<TodoList>>? GetOrderByExpression(
        PaginatedQueryDto queryIn)
    {
        var keySelector = GetOrderByKeySelector(queryIn);

        if (keySelector is null)
        {
            return null;
        }

        if (queryIn.IsDescendingOrder)
        {
            return q => q.OrderByDescending(keySelector);
        }

        return q => q.OrderBy(keySelector);
    }

    private static Expression<Func<TodoList, object>>? GetOrderByKeySelector(PaginatedQueryDto queryIn)
    {
        return queryIn.OrderBy?.ToLower() switch
        {
            PropertyKeyForName => e => e.Name,
            _ => null
        };
    }

    private static Expression<Func<TodoList, bool>>? GetFiltersExpression(PaginatedQueryDto queryIn)
    {
        if (string.IsNullOrWhiteSpace(queryIn.FilterBy) || string.IsNullOrWhiteSpace(queryIn.FilterValue))
        {
            return null;
        }

        return queryIn.FilterBy.ToLower() switch
        {
            PropertyKeyForName => e => e.Name.Contains(queryIn.FilterValue),
            PropertyKeyForOwnerId => e => e.OwnerId == queryIn.FilterValue,
            _ => null
        };
    }
}