using System.Diagnostics.CodeAnalysis;

namespace Todo.Application.Dtos.TodoLists.TodoItems.Create;

/// <summary>
/// The create to do item DTO.
/// </summary>
[ExcludeFromCodeCoverage]
public record CreateTodoItemDto
{
    /// <summary>
    /// The identifier of the list.
    /// </summary>
    public Guid ListId { get; init; }

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
    public TodoItemScheduleDto? Schedule { get; init; }
}