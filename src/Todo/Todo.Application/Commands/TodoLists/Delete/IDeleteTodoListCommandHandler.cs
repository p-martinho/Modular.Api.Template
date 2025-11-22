using SharedCore.Application.Commands;
using Todo.Application.Dtos.TodoLists;

namespace Todo.Application.Commands.TodoLists.Delete;

/// <summary>
/// The delete to do list command handler.
/// </summary>
/// <seealso cref="ICommandHandler{TIn,TOutData}"/>
public interface IDeleteTodoListCommandHandler : ICommandHandler<Guid, TodoListDto>;