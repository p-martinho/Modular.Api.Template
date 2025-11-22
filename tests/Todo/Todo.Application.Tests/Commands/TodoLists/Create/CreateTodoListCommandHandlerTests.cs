using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using SharedCore.Application.Common.Models;
using SharedCore.Common.ApplicationContext;
using Todo.Application.Commands.TodoLists.Create;
using Todo.Application.Dtos.TodoLists.Create;
using Todo.Domain.Entities.TodoLists;
using Todo.Persistence.Repositories.TodoLists;

namespace Todo.Application.Tests.Commands.TodoLists.Create;

public class CreateTodoListCommandHandlerTests
{
    private readonly CreateTodoListCommandHandler _commandHandler;
    private readonly ITodoListRepository _repository;
    private readonly ICurrentUser _currentUser;

    public CreateTodoListCommandHandlerTests()
    {
        _repository = Substitute.For<ITodoListRepository>();

        _currentUser = Substitute.For<ICurrentUser>();
        _currentUser.UserId.Returns("UserId");

        _commandHandler = new CreateTodoListCommandHandler(
            NullLogger<CreateTodoListCommandHandler>.Instance,
            null!,
            _repository,
            _currentUser);
    }

    [Fact]
    public async Task HandleAsync_ShouldSucceed()
    {
        // Arrange
        var commandIn = new CreateTodoListDto { Name = "Name" };

        // Act
        var result = await _commandHandler.HandleAsync(commandIn, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.Success, result.Result.ResultType);
        Assert.NotNull(result.Data);
        Assert.Equal(commandIn.Name, result.Data.Name);
        await _repository.Received(1)
            .AddAsync(Arg.Is<TodoList>(t => t.Name == commandIn.Name), Arg.Any<CancellationToken>());
        await _repository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenUserIsNotAuthenticated_ShouldReturnAuthorizationError()
    {
        // Arrange
        var commandIn = new CreateTodoListDto { Name = "Name" };
        _currentUser.UserId.Returns((string?)null);

        // Act
        var result = await _commandHandler.HandleAsync(commandIn, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.AuthorizationError, result.Result.ResultType);
        Assert.Null(result.Data);
        await _repository.DidNotReceiveWithAnyArgs().AddAsync(null!, CancellationToken.None);
    }

    [Fact]
    public async Task HandleAsync_WhenInvalidInput_ShouldReturnValidationError()
    {
        // Arrange
        var commandIn = new CreateTodoListDto { Name = null! };

        // Act
        var result = await _commandHandler.HandleAsync(commandIn, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.ValidationError, result.Result.ResultType);
        Assert.Null(result.Data);
        await _repository.DidNotReceiveWithAnyArgs().AddAsync(null!, CancellationToken.None);
    }
}