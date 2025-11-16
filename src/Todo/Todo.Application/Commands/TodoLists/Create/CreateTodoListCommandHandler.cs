using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedCore.Application.Commands;
using SharedCore.Application.Commands.Models;
using SharedCore.Common.ApplicationContext;
using Todo.Application.Dtos.TodoLists;
using Todo.Application.Dtos.TodoLists.Create;
using Todo.Application.MappingExtensions.TodoLists;
using Todo.Domain.Entities.TodoLists;
using Todo.Persistence.Repositories.TodoLists;

namespace Todo.Application.Commands.TodoLists.Create;

/// <summary>
/// The create to do list command handler.
/// </summary>
/// <seealso cref="CommandHandler{TIn,TOutData}"/>
/// <seealso cref="ICreateTodoListCommandHandler"/>
internal class CreateTodoListCommandHandler : CommandHandler<CreateTodoListDto, TodoListDto>,
    ICreateTodoListCommandHandler
{
    private readonly ITodoListRepository _repository;
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateTodoListCommandHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="validator">The validator.</param>
    /// <param name="repository">The to do list repository.</param>
    /// <param name="currentUser">The current user.</param>
    public CreateTodoListCommandHandler(ILogger<CreateTodoListCommandHandler> logger,
        IValidator<CreateTodoListDto> validator,
        ITodoListRepository repository,
        ICurrentUser currentUser)
        : base(logger, validator)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    /// <inheritdoc />
    protected override async Task<CommandOut<TodoListDto>> HandleCommandInAsync(CreateTodoListDto commandIn,
        CancellationToken cancellationToken)
    {
        var ownerId = _currentUser.UserId;

        if (string.IsNullOrWhiteSpace(ownerId))
        {
            Logger.LogUnauthenticatedAttempt(commandIn);

            return CommandOut<TodoListDto>.AuthorizationError();
        }

        var todoList = TodoList.Create(ownerId, commandIn.Name);

        if (todoList is null)
        {
            return CommandOut<TodoListDto>.ValidationError();
        }

        await _repository.AddAsync(todoList, cancellationToken);

        await _repository.SaveChangesAsync(cancellationToken);

        return CommandOut<TodoListDto>.Success(todoList.ToDto());
    }
}