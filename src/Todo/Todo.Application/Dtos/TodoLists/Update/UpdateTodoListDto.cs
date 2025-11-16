using System.Diagnostics.CodeAnalysis;

namespace Todo.Application.Dtos.TodoLists.Update;

/// <summary>
/// The update to do list DTO.
/// </summary>
[ExcludeFromCodeCoverage]
public record UpdateTodoListDto
{
    /// <summary>
    /// The identifier of the to do list.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// The name of the list.
    /// </summary>
    public string? Name { get; init; }
}