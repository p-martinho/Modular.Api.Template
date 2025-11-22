using System.Diagnostics.CodeAnalysis;

namespace Todo.Application.Dtos.TodoLists.Create;

/// <summary>
/// The create to do list DTO.
/// </summary>
[ExcludeFromCodeCoverage]
public record CreateTodoListDto
{
    /// <summary>
    /// The name of the list.
    /// </summary>
    public required string Name { get; init; }
}