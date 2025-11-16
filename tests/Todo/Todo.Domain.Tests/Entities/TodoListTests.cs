using Todo.Domain.Entities.TodoLists;

namespace Todo.Domain.Tests.Entities;

public class TodoListTests
{
    [Fact]
    public void Create_ShouldSucceed()
    {
        // Arrange
        const string ownerId = "OwnerId";
        const string name = "Name";

        // Act
        var result = TodoList.Create(ownerId, name);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ownerId, result.OwnerId);
        Assert.Equal(name, result.Name);
        Assert.NotNull(result.Items);
        Assert.Empty(result.Items);
    }

    [Theory]
    [InlineData(null, "Name")]
    [InlineData("", "Name")]
    [InlineData(" ", "Name")]
    [InlineData("OwnerId", null)]
    [InlineData("OwnerId", "")]
    [InlineData("OwnerId", " ")]
    public void Create_WhenInvalidInput_ShouldReturnNull(string? ownerId, string? name)
    {
        // Arrange

        // Act
        var result = TodoList.Create(ownerId!, name!);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void UpdateName_ShouldSucceed()
    {
        // Arrange
        const string newName = "NewName";
        var todoList = TodoList.Create("OwnerId", "Name")!;

        // Act
        var result = todoList.UpdateName(newName);

        // Assert
        Assert.True(result);
        Assert.Equal(newName, todoList.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void UpdateName_WhenInvalidNewName_ShouldFail(string? newName)
    {
        // Arrange
        const string originalName = "Name";
        var todoList = TodoList.Create("OwnerId", originalName)!;

        // Act
        var result = todoList.UpdateName(newName!);

        // Assert
        Assert.False(result);
        Assert.Equal(originalName, todoList.Name);
    }

    [Fact]
    public void AddTodoItem_ShouldSucceed()
    {
        // Arrange
        const string todoItemTitle = "TodoItemTitle";
        const string todoItemDescription = "TodoItemDescription";
        var todoList = TodoList.Create("OwnerId", "Name")!;

        // Act
        var result = todoList.AddTodoItem(todoItemTitle, todoItemDescription);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(todoItemTitle, result.Title);
        Assert.Equal(todoItemDescription, result.Description);
        Assert.Equal(todoList.Id, result.TodoListId);
        Assert.Single(todoList.Items);
        Assert.Equal(todoList.Items.First(), result);
    }

    [Fact]
    public void AddTodoItem_WhenDescriptionIsNull_ShouldSucceed()
    {
        // Arrange
        var todoList = TodoList.Create("OwnerId", "Name")!;

        // Act
        var result = todoList.AddTodoItem("TodoItemTitle", description: null);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Description);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void AddTodoItem_WhenInvalidTitle_ShouldFailAndReturnNull(string? todoItemTitle)
    {
        // Arrange
        var todoList = TodoList.Create("OwnerId", "Name")!;

        // Act
        var result = todoList.AddTodoItem(todoItemTitle!, description: null);

        // Assert
        Assert.Null(result);
        Assert.Empty(todoList.Items);
    }

    [Fact]
    public void RemoveTodoItem_ShouldSucceed()
    {
        // Arrange
        var todoList = TodoList.Create("OwnerId", "Name")!;
        var todoItem = todoList.AddTodoItem("TodoItemTitle", null)!;

        // Act
        var result = todoList.RemoveTodoItem(todoItem.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(todoItem, result);
        Assert.Empty(todoList.Items);
    }

    [Fact]
    public void RemoveTodoItem_WhenEmptyId_ShouldFailAndReturnNull()
    {
        // Arrange
        var todoList = TodoList.Create("OwnerId", "Name")!;
        todoList.AddTodoItem("TodoItemTitle", null);

        // Act
        var result = todoList.RemoveTodoItem(Guid.Empty);

        // Assert
        Assert.Null(result);
        Assert.Single(todoList.Items);
    }

    [Fact]
    public void RemoveTodoItem_WhenTodoItemNotFound_ShouldFailAndReturnNull()
    {
        // Arrange
        var todoList = TodoList.Create("OwnerId", "Name")!;
        todoList.AddTodoItem("TodoItemTitle", null);

        // Act
        var result = todoList.RemoveTodoItem(Guid.NewGuid());

        // Assert
        Assert.Null(result);
        Assert.Single(todoList.Items);
    }
}