using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using SharedCore.Application.Common.Models;
using Todo.Application.Queries.TodoLists.GetById;
using Todo.Domain.Entities.TodoLists;
using Todo.Persistence.Repositories.TodoLists;

namespace Todo.Application.Tests.Queries.TodoLists.GetById;

public class GetTodoListByIdQueryHandlerTests
{
    private readonly GetTodoListByIdQueryHandler _queryHandler;
    private readonly ITodoListRepository _repository;

    public GetTodoListByIdQueryHandlerTests()
    {
        _repository = Substitute.For<ITodoListRepository>();

        _queryHandler = new GetTodoListByIdQueryHandler(
            NullLogger<GetTodoListByIdQueryHandler>.Instance,
            _repository);
    }

    [Fact]
    public async Task HandleAsync_ShouldSucceed()
    {
        // Arrange
        var todoList = TodoList.Create(Guid.NewGuid().ToString(), "Name")!;
        var id = todoList.Id;
        _repository.GetByIdAsync(id, isToDisableEntityTracking: true, cancellationToken: Arg.Any<CancellationToken>()).Returns(todoList);

        // Act
        var result = await _queryHandler.HandleAsync(id, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.Success, result.Result.ResultType);
        Assert.NotNull(result.Data);
        Assert.Equal(todoList.Name, result.Data.Name);
    }

    [Fact]
    public async Task HandleAsync_WhenIdIsEmpty_ShouldReturnNotFound()
    {
        // Arrange
        var id = Guid.Empty;

        // Act
        var result = await _queryHandler.HandleAsync(id, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.NotFoundError, result.Result.ResultType);
        Assert.Null(result.Data);
        await _repository.DidNotReceiveWithAnyArgs().GetByIdAsync(Guid.Empty, cancellationToken: CancellationToken.None);
    }

    [Fact]
    public async Task HandleAsync_WhenNotFound_ShouldReturnNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repository.GetByIdAsync(id, cancellationToken: Arg.Any<CancellationToken>()).Returns((TodoList?)null);

        // Act
        var result = await _queryHandler.HandleAsync(id, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.NotFoundError, result.Result.ResultType);
        Assert.Null(result.Data);
        await _repository.Received(1).GetByIdAsync(id, isToDisableEntityTracking: true, cancellationToken: Arg.Any<CancellationToken>());
    }
}