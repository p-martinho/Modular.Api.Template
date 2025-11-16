using Microsoft.Extensions.Logging;
using SharedCore.Application.Queries;
using SharedCore.Application.Queries.Models;
using Todo.Application.Dtos.TodoLists;
using Todo.Application.MappingExtensions.TodoLists;
using Todo.Persistence.Repositories.TodoLists;

namespace Todo.Application.Queries.TodoLists.GetById;

/// <summary>
/// The get to do list by id query handler.
/// </summary>
/// <seealso cref="IGetTodoListByIdQueryHandler"/>
internal class GetTodoListByIdQueryHandler : QueryHandler<Guid, TodoListDto>, IGetTodoListByIdQueryHandler
{
    private readonly ITodoListRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetTodoListByIdQueryHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="repository">The to do list repository.</param>
    public GetTodoListByIdQueryHandler(ILogger<GetTodoListByIdQueryHandler> logger,
        ITodoListRepository repository)
        : base(logger)
    {
        _repository = repository;
    }

    /// <inheritdoc />
    protected override async Task<QueryOut<TodoListDto>> HandleQueryInAsync(Guid queryIn,
        CancellationToken cancellationToken)
    {
        if (queryIn == Guid.Empty)
        {
            return QueryOut<TodoListDto>.NotFoundError();
        }

        var todoList = await _repository.GetByIdAsync(queryIn, isToDisableEntityTracking: true,
            cancellationToken: cancellationToken);

        return todoList is null
            ? QueryOut<TodoListDto>.NotFoundError()
            : QueryOut<TodoListDto>.Success(todoList.ToDto());
    }
}