using Todo.Domain.Entities.TodoLists;

namespace Todo.Domain.Tests.Entities;

public class TodoItemTests
{
    [Fact]
    public void Constructor_ShouldReturnEntityWithDefaultValues()
    {
        // Arrange
        var listId = Guid.NewGuid();
        const string title = "Title";
        const string description = "Description";

        // Act
        var result = new TodoItem(listId, title, description);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(listId, result.TodoListId);
        Assert.Equal(title, result.Title);
        Assert.Equal(description, result.Description);
        Assert.False(result.IsDone);
        Assert.NotNull(result.Schedule);
        Assert.Null(result.Schedule.DueDate);
    }

    [Fact]
    public void UpdateTitle_ShouldSucceed()
    {
        // Arrange
        const string newTitle = "NewTitle";
        var todoItem = new TodoItem(Guid.NewGuid(), "Title", "Description");

        // Act
        var result = todoItem.UpdateTitle(newTitle);

        // Assert
        Assert.True(result);
        Assert.Equal(newTitle, todoItem.Title);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void UpdateTitle_WhenInvalidNewTitle_ShouldFail(string? newTitle)
    {
        // Arrange
        const string originalTitle = "Title";
        var todoItem = new TodoItem(Guid.NewGuid(), originalTitle, "Description");

        // Act
        var result = todoItem.UpdateTitle(newTitle!);

        // Assert
        Assert.False(result);
        Assert.Equal(originalTitle, todoItem.Title);
    }

    [Fact]
    public void UpdateTitle_WhenNewTitleIsEqualToOriginalTitle_ShouldFail()
    {
        // Arrange
        const string originalTitle = "Title";
        var todoItem = new TodoItem(Guid.NewGuid(), originalTitle, "Description");

        // Act
        var result = todoItem.UpdateTitle(originalTitle);

        // Assert
        Assert.False(result);
        Assert.Equal(originalTitle, todoItem.Title);
    }

    [Fact]
    public void ScheduleTo_ShouldSucceed()
    {
        // Arrange
        var dueDate = DateTimeOffset.UtcNow.AddDays(1);
        var todoItem = new TodoItem(Guid.NewGuid(), "Title", "Description");

        // Act
        var result = todoItem.ScheduleTo(dueDate);

        // Assert
        Assert.True(result);
        Assert.NotNull(todoItem.Schedule);
        Assert.Equal(dueDate, todoItem.Schedule.DueDate);
    }

    [Fact]
    public void ScheduleTo_WhenDueDateIsInThePast_ShouldFail()
    {
        // Arrange
        var dueDate = DateTimeOffset.UtcNow.AddSeconds(-1);
        var todoItem = new TodoItem(Guid.NewGuid(), "Title", "Description");

        // Act
        var result = todoItem.ScheduleTo(dueDate);

        // Assert
        Assert.False(result);
        Assert.NotNull(todoItem.Schedule);
        Assert.Null(todoItem.Schedule.DueDate);
    }

    [Fact]
    public void ClearSchedule_ShouldSucceed()
    {
        // Arrange
        var dueDate = DateTimeOffset.UtcNow.AddDays(1);
        var todoItem = new TodoItem(Guid.NewGuid(), "Title", "Description");

        // Act
        var result1 = todoItem.ScheduleTo(dueDate);
        var result2 = todoItem.ClearSchedule();

        // Assert
        Assert.True(result1);
        Assert.True(result2);
        Assert.NotNull(todoItem.Schedule);
        Assert.Null(todoItem.Schedule.DueDate);
    }
}