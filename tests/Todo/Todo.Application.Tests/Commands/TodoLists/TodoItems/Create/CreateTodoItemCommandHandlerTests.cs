using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using SharedCore.Application.Common.Models;
using Todo.Application.Commands.TodoLists.TodoItems.Create;
using Todo.Application.Dtos.TodoLists.TodoItems;
using Todo.Application.Dtos.TodoLists.TodoItems.Create;
using Todo.Domain.Entities.TodoLists;
using Todo.Persistence.Repositories.TodoLists;

namespace Todo.Application.Tests.Commands.TodoLists.TodoItems.Create;

public class CreateTodoItemCommandHandlerTests
{
    private readonly CreateTodoItemCommandHandler _commandHandler;
    private readonly ITodoListRepository _repository;

    public CreateTodoItemCommandHandlerTests()
    {
        _repository = Substitute.For<ITodoListRepository>();

        _commandHandler = new CreateTodoItemCommandHandler(
            NullLogger<CreateTodoItemCommandHandler>.Instance,
            null!,
            _repository);
    }

    [Fact]
    public async Task HandleAsync_ShouldSucceed()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var commandIn = new CreateTodoItemDto
        {
            ListId = listId,
            Title = "Title",
            Description = "Description",
            Schedule = new TodoItemScheduleDto(DateTimeOffset.UtcNow.AddDays(1))
        };
        var todoList = TodoList.Create("OwnerId", "Name")!;
        _repository.GetByIdAsync(listId, cancellationToken: Arg.Any<CancellationToken>()).Returns(todoList);

        // Act
        var result = await _commandHandler.HandleAsync(commandIn, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.Success, result.Result.ResultType);
        Assert.NotNull(result.Data);
        Assert.Equal(commandIn.Title, result.Data.Title);
        Assert.Equal(commandIn.Description, result.Data.Description);
        Assert.Equal(commandIn.Schedule.DueDate, result.Data.Schedule.DueDate);
        await _repository.Received(1).GetByIdAsync(listId, cancellationToken: Arg.Any<CancellationToken>());
        await _repository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        Assert.Single(todoList.Items);
        var todoItem = todoList.Items.Single();
        Assert.Equal(commandIn.Title, todoItem.Title);
        Assert.Equal(commandIn.Description, todoItem.Description);
        Assert.Equal(commandIn.Schedule.DueDate, todoItem.Schedule.DueDate);
        Assert.False(todoItem.IsDone);
    }

    [Fact]
    public async Task HandleAsync_WhenIdIsEmpty_ShouldReturnNotFound()
    {
        // Arrange
        var listId = Guid.Empty;
        var commandIn = new CreateTodoItemDto { ListId = listId, Title = "Title" };

        // Act
        var result = await _commandHandler.HandleAsync(commandIn, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.NotFoundError, result.Result.ResultType);
        Assert.Null(result.Data);
        await _repository.DidNotReceiveWithAnyArgs()
            .GetByIdAsync(Guid.Empty, cancellationToken: CancellationToken.None);
    }

    [Fact]
    public async Task HandleAsync_WhenListNotFound_ShouldReturnNotFound()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var commandIn = new CreateTodoItemDto { ListId = listId, Title = "Title" };
        _repository.GetByIdAsync(listId, cancellationToken: Arg.Any<CancellationToken>()).Returns((TodoList?)null);

        // Act
        var result = await _commandHandler.HandleAsync(commandIn, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.NotFoundError, result.Result.ResultType);
        Assert.Null(result.Data);
        await _repository.Received(1).GetByIdAsync(listId, cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenInvalidInput_ShouldReturnValidationError()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var commandIn = new CreateTodoItemDto { ListId = listId, Title = null! };
        var todoList = TodoList.Create("OwnerId", "Name")!;
        _repository.GetByIdAsync(listId, cancellationToken: Arg.Any<CancellationToken>()).Returns(todoList);

        // Act
        var result = await _commandHandler.HandleAsync(commandIn, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.ValidationError, result.Result.ResultType);
        Assert.Null(result.Data);
        await _repository.DidNotReceiveWithAnyArgs().AddAsync(null!, CancellationToken.None);
    }
}