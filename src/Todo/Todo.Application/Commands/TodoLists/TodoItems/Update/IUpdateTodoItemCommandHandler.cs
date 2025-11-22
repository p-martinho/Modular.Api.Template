using SharedCore.Application.Commands;
using Todo.Application.Dtos.TodoLists.TodoItems;
using Todo.Application.Dtos.TodoLists.TodoItems.Update;

namespace Todo.Application.Commands.TodoLists.TodoItems.Update;

/// <summary>
/// The update to do item command handler.
/// </summary>
/// <seealso cref="ICommandHandler{TIn,TOutData}"/>
public interface IUpdateTodoItemCommandHandler : ICommandHandler<UpdateTodoItemDto, TodoItemDto>;