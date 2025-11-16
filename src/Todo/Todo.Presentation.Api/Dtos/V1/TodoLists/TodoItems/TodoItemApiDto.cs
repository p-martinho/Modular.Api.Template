using System.Diagnostics.CodeAnalysis;

namespace Todo.Presentation.Api.Dtos.V1.TodoLists.TodoItems;

/// <summary>
/// The to do item API DTO.
/// </summary>
[ExcludeFromCodeCoverage]
public record TodoItemApiDto
{
    /// <summary>
    /// The identifier.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// The item title.
    /// </summary>
    public string Title { get; init; } = null!;

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
    public required TodoItemScheduleApiDto Schedule { get; init; }
}