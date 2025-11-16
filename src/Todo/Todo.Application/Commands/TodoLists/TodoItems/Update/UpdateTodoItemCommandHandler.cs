using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedCore.Application.Commands;
using SharedCore.Application.Commands.Models;
using Todo.Application.Dtos.TodoLists.TodoItems;
using Todo.Application.Dtos.TodoLists.TodoItems.Update;
using Todo.Application.MappingExtensions.TodoLists;
using Todo.Domain.Entities.TodoLists;
using Todo.Persistence.Repositories.TodoLists;

namespace Todo.Application.Commands.TodoLists.TodoItems.Update;

/// <summary>
/// The update to do item command handler.
/// </summary>
/// <seealso cref="CommandHandler{TIn,TOutData}"/>
/// <seealso cref="IUpdateTodoItemCommandHandler"/>
internal class UpdateTodoItemCommandHandler : CommandHandler<UpdateTodoItemDto, TodoItemDto>,
    IUpdateTodoItemCommandHandler
{
    private readonly ITodoListRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateTodoItemCommandHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="validator">The validator.</param>
    /// <param name="repository">The to do list repository.</param>
    public UpdateTodoItemCommandHandler(ILogger<UpdateTodoItemCommandHandler> logger,
        IValidator<UpdateTodoItemDto> validator,
        ITodoListRepository repository)
        : base(logger, validator)
    {
        _repository = repository;
    }

    /// <inheritdoc />
    protected override async Task<CommandOut<TodoItemDto>> HandleCommandInAsync(UpdateTodoItemDto commandIn,
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

        var todoItem = todoList.Items.FirstOrDefault(e => e.Id == commandIn.Id);

        if (todoItem is null)
        {
            return CommandOut<TodoItemDto>.NotFoundError();
        }

        UpdateTodoItem(commandIn, todoItem);

        await _repository.SaveChangesAsync(cancellationToken);

        return CommandOut<TodoItemDto>.Success(todoItem.ToDto());
    }

    private static void UpdateTodoItem(UpdateTodoItemDto commandIn, TodoItem todoItem)
    {
        if (commandIn.Title is not null)
        {
            todoItem.UpdateTitle(commandIn.Title);
        }

        if (commandIn.Description is not null)
        {
            todoItem.Description = commandIn.Description;
        }

        if (commandIn.IsDone.HasValue)
        {
            todoItem.IsDone = commandIn.IsDone.Value;
        }

        if (commandIn.Schedule is not null)
        {
            if (commandIn.Schedule.DueDate.HasValue)
            {
                todoItem.ScheduleTo(commandIn.Schedule.DueDate.Value);
            }
            else
            {
                todoItem.ClearSchedule();
            }
        }
    }
}