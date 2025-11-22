using System.Diagnostics.CodeAnalysis;
using Todo.Application.Dtos.TodoLists.TodoItems;

namespace Todo.Application.Dtos.TodoLists;

/// <summary>
/// The to do list DTO.
/// </summary>
[ExcludeFromCodeCoverage]
public record TodoListDto
{
    /// <summary>
    /// The identifier.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// The name of the list.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// The collection of items of the list.
    /// </summary>
    public IReadOnlyCollection<TodoItemDto>? Items { get; init; }
}