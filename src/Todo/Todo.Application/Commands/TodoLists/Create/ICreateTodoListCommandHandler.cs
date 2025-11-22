using SharedCore.Application.Commands;
using Todo.Application.Dtos.TodoLists;
using Todo.Application.Dtos.TodoLists.Create;

namespace Todo.Application.Commands.TodoLists.Create;

/// <summary>
/// The create to do list command handler.
/// </summary>
/// <seealso cref="ICommandHandler{TIn,TOutData}"/>
public interface ICreateTodoListCommandHandler : ICommandHandler<CreateTodoListDto, TodoListDto>;