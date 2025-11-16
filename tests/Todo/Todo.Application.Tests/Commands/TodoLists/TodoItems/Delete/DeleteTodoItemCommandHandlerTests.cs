using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using SharedCore.Application.Common.Models;
using Todo.Application.Commands.TodoLists.TodoItems.Delete;
using Todo.Application.Dtos.TodoLists.TodoItems.Delete;
using Todo.Domain.Entities.TodoLists;
using Todo.Persistence.Repositories.TodoLists;

namespace Todo.Application.Tests.Commands.TodoLists.TodoItems.Delete;

public class DeleteTodoItemCommandHandlerTests
{
    private readonly DeleteTodoItemCommandHandler _commandHandler;
    private readonly ITodoListRepository _repository;

    public DeleteTodoItemCommandHandlerTests()
    {
        _repository = Substitute.For<ITodoListRepository>();

        _commandHandler = new DeleteTodoItemCommandHandler(
            NullLogger<DeleteTodoItemCommandHandler>.Instance,
            _repository);
    }

    [Fact]
    public async Task HandleAsync_ShouldSucceed()
    {
        // Arrange
        var todoList = TodoList.Create("OwnerId", "Name")!;
        var todoItem = todoList.AddTodoItem("Title", null)!;
        var commandIn = new DeleteTodoItemDto { Id = todoItem.Id, ListId = todoList.Id };
        _repository.GetByIdAsync(commandIn.ListId, cancellationToken: Arg.Any<CancellationToken>()).Returns(todoList);

        // Act
        var result = await _commandHandler.HandleAsync(commandIn, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.Success, result.Result.ResultType);
        Assert.NotNull(result.Data);
        Assert.Equal(todoItem.Title, result.Data.Title);
        Assert.Empty(todoList.Items);
        await _repository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenListIdIsEmpty_ShouldReturnNotFound()
    {
        // Arrange
        var commandIn = new DeleteTodoItemDto { Id = Guid.NewGuid(), ListId = Guid.Empty };

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
        var commandIn = new DeleteTodoItemDto { Id = Guid.Empty, ListId = Guid.NewGuid() };

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
    public async Task HandleAsync_WhenTodoListNotFound_ShouldReturnNotFound()
    {
        // Arrange
        var commandIn = new DeleteTodoItemDto { Id = Guid.NewGuid(), ListId = Guid.NewGuid() };
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
        var commandIn = new DeleteTodoItemDto { Id = Guid.NewGuid(), ListId = todoList.Id };
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