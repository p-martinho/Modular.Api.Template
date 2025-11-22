using SharedCore.Domain.Entities;
using Todo.Domain.ValueObjects.TodoLists;

namespace Todo.Domain.Entities.TodoLists;

/// <summary>
/// The to do item.
/// </summary>
/// <seealso cref="BaseAuditableEntity"/>
public sealed class TodoItem : BaseAuditableEntity
{
    internal TodoItem(Guid todoListId, string title, string? description)
    {
        TodoListId = todoListId;
        Title = title;
        Description = description;
        IsDone = false;
        Schedule = new TodoItemSchedule(DueDate: null);
    }

    /// <summary>
    /// The to do list identifier.
    /// </summary>
    public Guid TodoListId { get; private set; }

    /// <summary>
    /// The item title.
    /// </summary>
    public string Title { get; private set; }

    /// <summary>
    /// The item description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Value indicating whether the to do item is done.
    /// </summary>
    public bool IsDone { get; set; }

    /// <summary>
    /// The to do item schedule.
    /// </summary>
    public TodoItemSchedule Schedule { get; private set; }

    /// <summary>
    /// Updates the title of the to do item.
    /// </summary>
    /// <param name="newTitle">The new title for the to do item.</param>
    /// <returns><c>true</c> if succeeded; <c>false</c> otherwise.</returns>
    public bool UpdateTitle(string newTitle)
    {
        if (string.IsNullOrWhiteSpace(newTitle) || Title == newTitle)
        {
            return false;
        }

        Title = newTitle;

        return true;
    }

    /// <summary>
    /// Sets the schedule.
    /// </summary>
    /// <returns><c>true</c> if succeeded; <c>false</c> otherwise.</returns>
    public bool ScheduleTo(DateTimeOffset dueDate)
    {
        if (dueDate < DateTimeOffset.UtcNow)
        {
            return false;
        }

        Schedule = new TodoItemSchedule(dueDate);

        return true;
    }

    /// <summary>
    /// Clears the schedule.
    /// </summary>
    /// <returns><c>true</c> if succeeded; <c>false</c> otherwise.</returns>
    public bool ClearSchedule()
    {
        Schedule = new TodoItemSchedule(DueDate: null);

        return true;
    }
}