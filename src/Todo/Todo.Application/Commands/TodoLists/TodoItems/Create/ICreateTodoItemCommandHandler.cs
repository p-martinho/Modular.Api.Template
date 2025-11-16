using SharedCore.Application.Commands;
using Todo.Application.Dtos.TodoLists.TodoItems;
using Todo.Application.Dtos.TodoLists.TodoItems.Create;

namespace Todo.Application.Commands.TodoLists.TodoItems.Create;

/// <summary>
/// The create to do item command handler.
/// </summary>
/// <seealso cref="ICommandHandler{TIn,TOutData}"/>
public interface ICreateTodoItemCommandHandler : ICommandHandler<CreateTodoItemDto, TodoItemDto>;