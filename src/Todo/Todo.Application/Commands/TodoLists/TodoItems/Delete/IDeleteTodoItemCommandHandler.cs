using SharedCore.Application.Commands;
using Todo.Application.Dtos.TodoLists.TodoItems;
using Todo.Application.Dtos.TodoLists.TodoItems.Delete;

namespace Todo.Application.Commands.TodoLists.TodoItems.Delete;

/// <summary>
/// The delete to do item command handler.
/// </summary>
/// <seealso cref="ICommandHandler{TIn,TOutData}"/>
public interface IDeleteTodoItemCommandHandler : ICommandHandler<DeleteTodoItemDto, TodoItemDto>;