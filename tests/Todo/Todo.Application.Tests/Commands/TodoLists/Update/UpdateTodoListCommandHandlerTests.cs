using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using SharedCore.Application.Common.Models;
using Todo.Application.Commands.TodoLists.Update;
using Todo.Application.Dtos.TodoLists.Update;
using Todo.Domain.Entities.TodoLists;
using Todo.Persistence.Repositories.TodoLists;

namespace Todo.Application.Tests.Commands.TodoLists.Update;

public class UpdateTodoListCommandHandlerTests
{
    private readonly UpdateTodoListCommandHandler _commandHandler;
    private readonly ITodoListRepository _repository;

    public UpdateTodoListCommandHandlerTests()
    {
        _repository = Substitute.For<ITodoListRepository>();

        _commandHandler = new UpdateTodoListCommandHandler(
            NullLogger<UpdateTodoListCommandHandler>.Instance,
            null!,
            _repository);
    }

    [Fact]
    public async Task HandleAsync_ShouldSucceed()
    {
        // Arrange
        var id = Guid.NewGuid();
        var todoList = TodoList.Create("OwnerId", "Name")!;
        _repository.GetByIdAsync(id, cancellationToken: Arg.Any<CancellationToken>()).Returns(todoList);
        var commandIn = new UpdateTodoListDto { Id = id, Name = "UpdatedName" };

        // Act
        var result = await _commandHandler.HandleAsync(commandIn, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.Success, result.Result.ResultType);
        Assert.NotNull(result.Data);
        Assert.Equal(commandIn.Name, result.Data.Name);
        await _repository.Received(1).GetByIdAsync(id, cancellationToken: Arg.Any<CancellationToken>());
        await _repository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenIdIsEmpty_ShouldReturnNotFound()
    {
        // Arrange
        var commandIn = new UpdateTodoListDto { Id = Guid.Empty, Name = "Name" };

        // Act
        var result = await _commandHandler.HandleAsync(commandIn, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.NotFoundError, result.Result.ResultType);
        Assert.Null(result.Data);
        await _repository.DidNotReceiveWithAnyArgs()
            .GetByIdAsync(Guid.Empty, cancellationToken: CancellationToken.None);
    }

    [Fact]
    public async Task HandleAsync_WhenNotFound_ShouldReturnNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repository.GetByIdAsync(id, cancellationToken: Arg.Any<CancellationToken>()).Returns((TodoList?)null);
        var commandIn = new UpdateTodoListDto { Id = id, Name = "Name" };

        // Act
        var result = await _commandHandler.HandleAsync(commandIn, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.NotFoundError, result.Result.ResultType);
        Assert.Null(result.Data);
        await _repository.Received(1).GetByIdAsync(id, cancellationToken: Arg.Any<CancellationToken>());
    }
}