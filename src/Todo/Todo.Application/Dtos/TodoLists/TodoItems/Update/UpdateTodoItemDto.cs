using System.Diagnostics.CodeAnalysis;

namespace Todo.Application.Dtos.TodoLists.TodoItems.Update;

/// <summary>
/// The update to do item DTO.
/// </summary>
[ExcludeFromCodeCoverage]
public record UpdateTodoItemDto
{
    /// <summary>
    /// The identifier of the list.
    /// </summary>
    public Guid ListId { get; init; }

    /// <summary>
    /// The identifier of the to do item.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// The item title.
    /// </summary>
    public string? Title { get; init; }

    /// <summary>
    /// The item description.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Value indicating whether the to do item is done.
    /// </summary>
    public bool? IsDone { get; init; }

    /// <summary>
    /// The to do item schedule.
    /// </summary>
    public TodoItemScheduleDto? Schedule { get; init; }
}