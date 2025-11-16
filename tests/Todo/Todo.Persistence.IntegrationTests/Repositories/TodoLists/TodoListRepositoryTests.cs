using Microsoft.Extensions.Options;
using NSubstitute;
using SharedCore.Common.ApplicationContext;
using SharedCore.Common.Authorization;
using SharedCore.Persistence.Repositories.Settings;
using Todo.Domain.Entities.TodoLists;
using Todo.Persistence.IntegrationTests.Fixtures;
using Todo.Persistence.Repositories.TodoLists;

namespace Todo.Persistence.IntegrationTests.Repositories.TodoLists;

public class TodoListRepositoryTests
{
    private readonly ITodoListRepository _repository;
    private readonly ICurrentUser _currentUser;

    public TodoListRepositoryTests(EfCoreFixture fixture)
    {
        var queryParametersOptions = Substitute.For<IOptionsSnapshot<QueryParametersSettings>>();
        queryParametersOptions.Value.Returns(new QueryParametersSettings());

        _currentUser = Substitute.For<ICurrentUser>();

        _repository = new TodoListRepository(fixture.Context, queryParametersOptions, _currentUser);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public async Task GetByIdAsync_ShouldApplyPermissions(bool isAdmin, bool isOwner)
    {
        // Arrange
        var todoList = TodoList.Create(Guid.NewGuid().ToString(), "Name")!;
        await _repository.AddAsync(todoList, TestContext.Current.CancellationToken);
        await _repository.SaveChangesAsync(TestContext.Current.CancellationToken);
        if (isOwner)
        {
            _currentUser.UserId.Returns(todoList.OwnerId);
        }

        if (isAdmin)
        {
            _currentUser.IsInRole(UserRoles.Admin).Returns(true);
        }

        // Act
        var fetchedTodoList = await _repository.GetByIdAsync(todoList.Id, isToDisableEntityTracking: true,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        if (isAdmin || isOwner)
        {
            Assert.NotNull(fetchedTodoList);
        }
        else
        {
            Assert.Null(fetchedTodoList);
        }
    }

    [Fact]
    public async Task GetByIdAsync_ShouldIncludeDefaultAggregateIncludes()
    {
        // Arrange
        var todoList = TodoList.Create(Guid.NewGuid().ToString(), "Name")!;
        todoList.AddTodoItem("Title 1", "Description 1");
        todoList.AddTodoItem("Title 2", "Description 2");
        await _repository.AddAsync(todoList, TestContext.Current.CancellationToken);
        await _repository.SaveChangesAsync(TestContext.Current.CancellationToken);
        _currentUser.UserId.Returns(todoList.OwnerId);

        // Act
        var fetchedTodoList = await _repository.GetByIdAsync(todoList.Id, isToDisableEntityTracking: true,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(fetchedTodoList);
        Assert.NotNull(fetchedTodoList.Items);
        Assert.Equal(2, fetchedTodoList.Items.Count);
    }
}