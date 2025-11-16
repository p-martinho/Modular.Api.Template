using SharedCore.Application.Dtos;
using SharedCore.Application.Queries;
using Todo.Application.Dtos.TodoLists;

namespace Todo.Application.Queries.TodoLists.Get;

/// <summary>
/// The get to do lists query handler.
/// </summary>
/// <seealso cref="IQueryHandler{TIn,TOut}"/>
public interface IGetTodoListsQueryHandler : IQueryHandler<PaginatedQueryDto, PaginatedQueryResultDto<TodoListDto>>;