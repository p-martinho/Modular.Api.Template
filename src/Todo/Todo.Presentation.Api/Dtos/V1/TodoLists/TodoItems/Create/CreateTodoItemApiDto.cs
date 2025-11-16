using System.Diagnostics.CodeAnalysis;

namespace Todo.Presentation.Api.Dtos.V1.TodoLists.TodoItems.Create;

/// <summary>
/// The create to do item API DTO.
/// </summary>
[ExcludeFromCodeCoverage]
public record CreateTodoItemApiDto
{
    /// <summary>
    /// The item title.
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// The item description.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// The to do item schedule.
    /// </summary>
    public TodoItemScheduleApiDto? Schedule { get; init; }
}