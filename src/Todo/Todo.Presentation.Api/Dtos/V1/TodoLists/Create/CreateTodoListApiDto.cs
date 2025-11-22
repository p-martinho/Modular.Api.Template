using System.Diagnostics.CodeAnalysis;

namespace Todo.Presentation.Api.Dtos.V1.TodoLists.Create;

/// <summary>
/// The create to do list API DTO.
/// </summary>
[ExcludeFromCodeCoverage]
public record CreateTodoListApiDto
{
    /// <summary>
    /// The name of the list.
    /// </summary>
    public required string Name { get; init; }
}