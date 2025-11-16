using System.Diagnostics.CodeAnalysis;

namespace Todo.Application.Dtos.TodoLists.TodoItems.Delete;

/// <summary>
/// The delete to do item DTO.
/// </summary>
[ExcludeFromCodeCoverage]
public record DeleteTodoItemDto
{
    /// <summary>
    /// The identifier of the list.
    /// </summary>
    public Guid ListId { get; init; }

    /// <summary>
    /// The identifier of the to do item.
    /// </summary>
    public Guid Id { get; init; }
}