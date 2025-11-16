using Microsoft.Extensions.Logging;
using SharedCore.Application.Commands;
using SharedCore.Application.Commands.Models;
using Todo.Application.Dtos.TodoLists.TodoItems;
using Todo.Application.Dtos.TodoLists.TodoItems.Delete;
using Todo.Application.MappingExtensions.TodoLists;
using Todo.Persistence.Repositories.TodoLists;

namespace Todo.Application.Commands.TodoLists.TodoItems.Delete;

/// <summary>
/// The delete to do item command handler.
/// </summary>
/// <seealso cref="CommandHandler{TIn,TOutData}"/>
/// <seealso cref="IDeleteTodoItemCommandHandler"/>
internal class DeleteTodoItemCommandHandler : CommandHandler<DeleteTodoItemDto, TodoItemDto>,
    IDeleteTodoItemCommandHandler
{
    private readonly ITodoListRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteTodoItemCommandHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="repository">The to do list repository.</param>
    public DeleteTodoItemCommandHandler(ILogger<DeleteTodoItemCommandHandler> logger,
        ITodoListRepository repository)
        : base(logger)
    {
        _repository = repository;
    }

    /// <inheritdoc />
    protected override async Task<CommandOut<TodoItemDto>> HandleCommandInAsync(DeleteTodoItemDto commandIn,
        CancellationToken cancellationToken)
    {
        if (commandIn.ListId == Guid.Empty)
        {
            return CommandOut<TodoItemDto>.NotFoundError("List not found.");
        }

        if (commandIn.Id == Guid.Empty)
        {
            return CommandOut<TodoItemDto>.NotFoundError();
        }

        var todoList = await _repository.GetByIdAsync(commandIn.ListId, cancellationToken: cancellationToken);

        if (todoList is null)
        {
            return CommandOut<TodoItemDto>.NotFoundError("List not found.");
        }

        var todoItem = todoList.RemoveTodoItem(commandIn.Id);

        if (todoItem is null)
        {
            return CommandOut<TodoItemDto>.NotFoundError();
        }

        await _repository.SaveChangesAsync(cancellationToken);

        return CommandOut<TodoItemDto>.Success(todoItem.ToDto());
    }
}