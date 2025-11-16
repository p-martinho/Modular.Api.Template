using SharedCore.Application.Queries;
using Todo.Application.Dtos.TodoLists;

namespace Todo.Application.Queries.TodoLists.GetById;

/// <summary>
/// The get to do list by id query handler.
/// </summary>
/// <seealso cref="IQueryHandler{TIn,TOut}"/>
public interface IGetTodoListByIdQueryHandler : IQueryHandler<Guid, TodoListDto>;