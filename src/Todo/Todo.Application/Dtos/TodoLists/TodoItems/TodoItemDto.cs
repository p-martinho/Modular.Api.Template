using System.Diagnostics.CodeAnalysis;

namespace Todo.Application.Dtos.TodoLists.TodoItems;

/// <summary>
/// The to do item DTO.
/// </summary>
[ExcludeFromCodeCoverage]
public record TodoItemDto
{
    /// <summary>
    /// The identifier.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// The item title.
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// The item description.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Value indicating whether the to do item is done.
    /// </summary>
    public bool IsDone { get; init; }

    /// <summary>
    /// The to do item schedule.
    /// </summary>
    public required TodoItemScheduleDto Schedule { get; init; }
}