using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedCore.Application.Commands;
using SharedCore.Application.Commands.Models;
using Todo.Application.Dtos.TodoLists.TodoItems;
using Todo.Application.Dtos.TodoLists.TodoItems.Create;
using Todo.Application.MappingExtensions.TodoLists;
using Todo.Persistence.Repositories.TodoLists;

namespace Todo.Application.Commands.TodoLists.TodoItems.Create;

/// <summary>
/// The create to do item command handler.
/// </summary>
/// <seealso cref="CommandHandler{TIn,TOutData}"/>
/// <seealso cref="ICreateTodoItemCommandHandler"/>
internal class CreateTodoItemCommandHandler : CommandHandler<CreateTodoItemDto, TodoItemDto>,
    ICreateTodoItemCommandHandler
{
    private readonly ITodoListRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateTodoItemCommandHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="validator">The validator.</param>
    /// <param name="repository">The to do list repository.</param>
    public CreateTodoItemCommandHandler(ILogger<CreateTodoItemCommandHandler> logger,
        IValidator<CreateTodoItemDto> validator,
        ITodoListRepository repository)
        : base(logger, validator)
    {
        _repository = repository;
    }

    /// <inheritdoc />
    protected override async Task<CommandOut<TodoItemDto>> HandleCommandInAsync(CreateTodoItemDto commandIn,
        CancellationToken cancellationToken)
    {
        if (commandIn.ListId == Guid.Empty)
        {
            return CommandOut<TodoItemDto>.NotFoundError("List not found.");
        }

        var todoList = await _repository.GetByIdAsync(commandIn.ListId, cancellationToken: cancellationToken);

        if (todoList is null)
        {
            return CommandOut<TodoItemDto>.NotFoundError("List not found.");
        }

        var todoItem = todoList.AddTodoItem(commandIn.Title, commandIn.Description);

        if (todoItem is null)
        {
            return CommandOut<TodoItemDto>.ValidationError();
        }

        if (commandIn.Schedule?.DueDate is not null)
        {
            todoItem.ScheduleTo(commandIn.Schedule.DueDate.Value);
        }

        await _repository.SaveChangesAsync(cancellationToken);

        return CommandOut<TodoItemDto>.Success(todoItem.ToDto());
    }
}