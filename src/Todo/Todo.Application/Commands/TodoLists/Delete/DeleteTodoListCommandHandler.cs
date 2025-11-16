using Microsoft.Extensions.Logging;
using SharedCore.Application.Commands;
using SharedCore.Application.Commands.Models;
using Todo.Application.Dtos.TodoLists;
using Todo.Application.MappingExtensions.TodoLists;
using Todo.Persistence.Repositories.TodoLists;

namespace Todo.Application.Commands.TodoLists.Delete;

/// <summary>
/// The delete to do list command handler.
/// </summary>
/// <seealso cref="CommandHandler{TIn,TOutData}"/>
/// <seealso cref="IDeleteTodoListCommandHandler"/>
internal class DeleteTodoListCommandHandler : CommandHandler<Guid, TodoListDto>,
    IDeleteTodoListCommandHandler
{
    private readonly ITodoListRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteTodoListCommandHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="repository">The to do list repository.</param>
    public DeleteTodoListCommandHandler(ILogger<DeleteTodoListCommandHandler> logger,
        ITodoListRepository repository)
        : base(logger)
    {
        _repository = repository;
    }

    /// <inheritdoc />
    protected override async Task<CommandOut<TodoListDto>> HandleCommandInAsync(Guid commandIn,
        CancellationToken cancellationToken)
    {
        if (commandIn == Guid.Empty)
        {
            return CommandOut<TodoListDto>.NotFoundError();
        }

        var todoList = await _repository.GetByIdAsync(commandIn, isToDisableAggregateIncludes: true,
            cancellationToken: cancellationToken);

        if (todoList is null)
        {
            return CommandOut<TodoListDto>.NotFoundError();
        }

        await _repository.RemoveAsync(todoList, cancellationToken);

        await _repository.SaveChangesAsync(cancellationToken);

        return CommandOut<TodoListDto>.Success(todoList.ToDto());
    }
}