using System.Diagnostics.CodeAnalysis;
using Todo.Application.Dtos.TodoLists;
using Todo.Domain.Entities.TodoLists;

namespace Todo.Application.MappingExtensions.TodoLists;

/// <summary>
/// The to do list mapping extensions.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class TodoListMappingExtensions
{
    /// <summary>
    /// Converts the entity into a DTO.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <returns>The DTO.</returns>
    public static TodoListDto ToDto(this TodoList entity)
    {
        return new TodoListDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Items = entity.Items.Select(i => i.ToDto()).ToList().AsReadOnly()
        };
    }
}