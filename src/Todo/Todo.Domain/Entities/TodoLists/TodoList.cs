using SharedCore.Domain.Abstractions;
using SharedCore.Domain.Entities;

namespace Todo.Domain.Entities.TodoLists;

/// <summary>
/// The to do list.
/// </summary>
/// <seealso cref="BaseOwnedEntity"/>
/// <seealso cref="IAggregateEntity"/>
/// <seealso cref="ISoftDeletableEntity"/>
public sealed class TodoList : BaseOwnedEntity, IAggregateEntity, ISoftDeletableEntity
{
    private readonly List<TodoItem> _items = [];

    private TodoList()
    {
    }

    /// <summary>
    /// The name of the list.
    /// </summary>
    public string Name { get; private set; } = null!;

    /// <summary>
    /// The collection of items of the list.
    /// </summary>
    public IReadOnlyCollection<TodoItem> Items => _items.AsReadOnly();

    /// <summary>
    /// Creates a new to do list.
    /// </summary>
    /// <param name="ownerId">The owner identifier.</param>
    /// <param name="name">The name for the to do list.</param>
    /// <returns>The created to do list; or <c>null</c> in case of validation failure.</returns>
    public static TodoList? Create(string ownerId, string name)
    {
        if (string.IsNullOrWhiteSpace(ownerId) || string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        return new TodoList { OwnerId = ownerId, Name = name };
    }

    /// <summary>
    /// Updates the name of the to do list.
    /// </summary>
    /// <param name="newName">The new name for the to do list.</param>
    /// <returns><c>true</c> if succeeded; <c>false</c> otherwise.</returns>
    public bool UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName) || Name == newName)
        {
            return false;
        }

        Name = newName;

        return true;
    }

    /// <summary>
    /// Adds a to do item to the list.
    /// </summary>
    /// <param name="title">The to do item title.</param>
    /// <param name="description">The to do item description.</param>
    /// <returns>The created to do item; or <c>null</c> in case of validation failure.</returns>
    public TodoItem? AddTodoItem(string title, string? description)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return null;
        }

        var todoItem = new TodoItem(Id, title, description);

        _items.Add(todoItem);

        return todoItem;
    }

    /// <summary>
    /// Removes a to do item from the list, by its identifier.
    /// </summary>
    /// <param name="todoItemId">The to do item identifier.</param>
    /// <returns>The removed to do item; or <c>null</c> if not found.</returns>
    public TodoItem? RemoveTodoItem(Guid todoItemId)
    {
        if (todoItemId == Guid.Empty)
        {
            return null;
        }

        var todoItem = _items.FirstOrDefault(e => e.Id == todoItemId);

        if (todoItem is not null)
        {
            _items.Remove(todoItem);
        }

        return todoItem;
    }
}