using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using SharedCore.Application.Common.Models;
using Todo.Application.Commands.TodoLists.TodoItems.Update;
using Todo.Application.Dtos.TodoLists.TodoItems;
using Todo.Application.Dtos.TodoLists.TodoItems.Update;
using Todo.Domain.Entities.TodoLists;
using Todo.Persistence.Repositories.TodoLists;

namespace Todo.Application.Tests.Commands.TodoLists.TodoItems.Update;

public class UpdateTodoItemCommandHandlerTests
{
    private readonly UpdateTodoItemCommandHandler _commandHandler;
    private readonly ITodoListRepository _repository;

    public UpdateTodoItemCommandHandlerTests()
    {
        _repository = Substitute.For<ITodoListRepository>();

        _commandHandler = new UpdateTodoItemCommandHandler(
            NullLogger<UpdateTodoItemCommandHandler>.Instance,
            null!,
            _repository);
    }

    [Fact]
    public async Task HandleAsync_ShouldSucceed()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var todoList = TodoList.Create("OwnerId", "Name")!;
        var todoItem = todoList.AddTodoItem("Title", null)!;
        var commandIn = new UpdateTodoItemDto
        {
            ListId = listId,
            Id = todoItem.Id,
            Title = "NewTitle",
            Description = "Description",
            IsDone = true,
            Schedule = new TodoItemScheduleDto(DateTimeOffset.UtcNow.AddDays(1))
        };
        _repository.GetByIdAsync(listId, cancellationToken: Arg.Any<CancellationToken>()).Returns(todoList);

        // Act
        var result = await _commandHandler.HandleAsync(commandIn, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.Success, result.Result.ResultType);
        Assert.NotNull(result.Data);
        Assert.Equal(commandIn.Title, result.Data.Title);
        Assert.Equal(commandIn.Description, result.Data.Description);
        Assert.Equal(commandIn.Schedule.DueDate, result.Data.Schedule.DueDate);
        Assert.Equal(commandIn.IsDone, result.Data.IsDone);
        await _repository.Received(1).GetByIdAsync(listId, cancellationToken: Arg.Any<CancellationToken>());
        await _repository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        Assert.Single(todoList.Items);
        Assert.Equal(commandIn.Title, todoItem.Title);
        Assert.Equal(commandIn.Description, todoItem.Description);
        Assert.Equal(commandIn.IsDone, todoItem.IsDone);
        Assert.Equal(commandIn.Schedule.DueDate, todoItem.Schedule.DueDate);
    }

    [Fact]
    public async Task HandleAsync_WhenScheduleIsDefinedButDueDateIsNull_ShouldClearSchedule()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var todoList = TodoList.Create("OwnerId", "Name")!;
        var todoItem = todoList.AddTodoItem("Title", null)!;
        var commandIn = new UpdateTodoItemDto
        {
            ListId = listId,
            Id = todoItem.Id,
            Schedule = new TodoItemScheduleDto(null)
        };
        _repository.GetByIdAsync(listId, cancellationToken: Arg.Any<CancellationToken>()).Returns(todoList);

        // Act
        var result = await _commandHandler.HandleAsync(commandIn, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.Success, result.Result.ResultType);
        Assert.NotNull(result.Data);
        Assert.Null(result.Data.Schedule.DueDate);
        Assert.Null(todoItem.Schedule.DueDate);
    }

    [Fact]
    public async Task HandleAsync_WhenListIdIsEmpty_ShouldReturnNotFound()
    {
        // Arrange
        var commandIn = new UpdateTodoItemDto { Id = Guid.NewGuid(), ListId = Guid.Empty };

        // Act
        var result = await _commandHandler.HandleAsync(commandIn, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.NotFoundError, result.Result.ResultType);
        Assert.Equal("List not found.", result.Result.Message);
        Assert.Null(result.Data);
        await _repository.DidNotReceiveWithAnyArgs()
            .GetByIdAsync(Guid.Empty, cancellationToken: CancellationToken.None);
    }

    [Fact]
    public async Task HandleAsync_WhenIdIsEmpty_ShouldReturnNotFound()
    {
        // Arrange
        var commandIn = new UpdateTodoItemDto { Id = Guid.Empty, ListId = Guid.NewGuid() };

        // Act
        var result = await _commandHandler.HandleAsync(commandIn, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.NotFoundError, result.Result.ResultType);
        Assert.Equal("Resource not found.", result.Result.Message);
        Assert.Null(result.Data);
        await _repository.DidNotReceiveWithAnyArgs()
            .GetByIdAsync(Guid.Empty, cancellationToken: CancellationToken.None);
    }

    [Fact]
    public async Task HandleAsync_WhenListNotFound_ShouldReturnNotFound()
    {
        // Arrange
        var commandIn = new UpdateTodoItemDto { Id = Guid.NewGuid(), ListId = Guid.NewGuid() };
        _repository.GetByIdAsync(commandIn.ListId, cancellationToken: Arg.Any<CancellationToken>())
            .Returns((TodoList?)null);

        // Act
        var result = await _commandHandler.HandleAsync(commandIn, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.NotFoundError, result.Result.ResultType);
        Assert.Equal("List not found.", result.Result.Message);
        Assert.Null(result.Data);
        await _repository.Received(1).GetByIdAsync(commandIn.ListId, cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenTodoItemNotInTheList_ShouldReturnNotFound()
    {
        // Arrange
        var todoList = TodoList.Create("OwnerId", "Name")!;
        var commandIn = new UpdateTodoItemDto { Id = Guid.NewGuid(), ListId = todoList.Id };
        _repository.GetByIdAsync(commandIn.ListId, cancellationToken: Arg.Any<CancellationToken>()).Returns(todoList);

        // Act
        var result = await _commandHandler.HandleAsync(commandIn, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.NotFoundError, result.Result.ResultType);
        Assert.Equal("Resource not found.", result.Result.Message);
        Assert.Null(result.Data);
        await _repository.Received(1).GetByIdAsync(commandIn.ListId, cancellationToken: Arg.Any<CancellationToken>());
    }
}