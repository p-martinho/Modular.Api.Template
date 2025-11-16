using System.Diagnostics.CodeAnalysis;
using Todo.Application.Dtos.TodoLists.TodoItems;
using Todo.Domain.Entities.TodoLists;
using Todo.Domain.ValueObjects.TodoLists;

namespace Todo.Application.MappingExtensions.TodoLists;

/// <summary>
/// The to do item mapping extensions.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class TodoItemMappingExtensions
{
    /// <summary>
    /// Converts the entity into a DTO.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <returns>The DTO.</returns>
    public static TodoItemDto ToDto(this TodoItem entity)
    {
        return new TodoItemDto
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            IsDone = entity.IsDone,
            Schedule = entity.Schedule.ToDto()
        };
    }

    private static TodoItemScheduleDto ToDto(this TodoItemSchedule objectValue)
    {
        return new TodoItemScheduleDto(objectValue.DueDate);
    }
}