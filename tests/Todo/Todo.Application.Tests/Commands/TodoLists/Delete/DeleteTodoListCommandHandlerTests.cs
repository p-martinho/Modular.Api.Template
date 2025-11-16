using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using SharedCore.Application.Common.Models;
using Todo.Application.Commands.TodoLists.Delete;
using Todo.Domain.Entities.TodoLists;
using Todo.Persistence.Repositories.TodoLists;

namespace Todo.Application.Tests.Commands.TodoLists.Delete;

public class DeleteTodoListCommandHandlerTests
{
    private readonly DeleteTodoListCommandHandler _commandHandler;
    private readonly ITodoListRepository _repository;

    public DeleteTodoListCommandHandlerTests()
    {
        _repository = Substitute.For<ITodoListRepository>();

        _commandHandler = new DeleteTodoListCommandHandler(
            NullLogger<DeleteTodoListCommandHandler>.Instance,
            _repository);
    }

    [Fact]
    public async Task HandleAsync_ShouldSucceed()
    {
        // Arrange
        var id = Guid.NewGuid();
        var todoList = TodoList.Create("OwnerId", "Name")!;
        _repository.GetByIdAsync(id, isToDisableAggregateIncludes: true,
            cancellationToken: Arg.Any<CancellationToken>()).Returns(todoList);

        // Act
        var result = await _commandHandler.HandleAsync(id, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.Success, result.Result.ResultType);
        Assert.NotNull(result.Data);
        Assert.Equal(todoList.Name, result.Data.Name);
        await _repository.Received(1).RemoveAsync(todoList, Arg.Any<CancellationToken>());
        await _repository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenIdIsEmpty_ShouldReturnNotFound()
    {
        // Arrange
        var id = Guid.Empty;

        // Act
        var result = await _commandHandler.HandleAsync(id, TestContext.Current.CancellationToken);

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

        // Act
        var result = await _commandHandler.HandleAsync(id, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.NotFoundError, result.Result.ResultType);
        Assert.Null(result.Data);
        await _repository.Received(1).GetByIdAsync(id, isToDisableAggregateIncludes: true,
            cancellationToken: Arg.Any<CancellationToken>());
    }
}