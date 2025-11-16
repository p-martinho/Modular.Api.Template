using System.Diagnostics.CodeAnalysis;

namespace Todo.Presentation.Api.Dtos.V1.TodoLists.Update;

/// <summary>
/// The update to do list API DTO.
/// </summary>
[ExcludeFromCodeCoverage]
public record UpdateTodoListApiDto
{
    /// <summary>
    /// The name of the list.
    /// </summary>
    public string? Name { get; init; }
}