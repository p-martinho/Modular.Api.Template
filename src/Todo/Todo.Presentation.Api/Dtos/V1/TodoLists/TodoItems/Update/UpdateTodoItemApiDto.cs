using System.Diagnostics.CodeAnalysis;

namespace Todo.Presentation.Api.Dtos.V1.TodoLists.TodoItems.Update;

/// <summary>
/// The update to do item API DTO.
/// </summary>
[ExcludeFromCodeCoverage]
public record UpdateTodoItemApiDto
{
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
    public TodoItemScheduleApiDto? Schedule { get; init; }
}