using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Todo.Presentation.Api.Dtos.V1.TodoLists.TodoItems;

namespace Todo.Presentation.Api.Dtos.V1.TodoLists;

/// <summary>
/// The to do list API DTO.
/// </summary>
[ExcludeFromCodeCoverage]
public record TodoListApiDto
{
    /// <summary>
    /// The identifier.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// The name of the list.
    /// </summary>
    public string Name { get; init; } = null!;

    /// <summary>
    /// The collection of items of the list.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyCollection<TodoItemApiDto>? Items { get; init; }
}