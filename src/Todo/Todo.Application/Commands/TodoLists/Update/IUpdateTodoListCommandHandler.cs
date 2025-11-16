using SharedCore.Application.Commands;
using Todo.Application.Dtos.TodoLists;
using Todo.Application.Dtos.TodoLists.Update;

namespace Todo.Application.Commands.TodoLists.Update;

/// <summary>
/// The update to do list command handler.
/// </summary>
/// <seealso cref="ICommandHandler{TIn,TOutData}"/>
public interface IUpdateTodoListCommandHandler : ICommandHandler<UpdateTodoListDto, TodoListDto>;