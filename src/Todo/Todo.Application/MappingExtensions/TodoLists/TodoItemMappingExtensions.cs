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
    /// The <see cref="TodoItem"/> extensions.
    /// </summary>
    /// <param name="entity">The entity.</param>
    extension(TodoItem entity)
    {
        /// <summary>
        /// Converts the entity into a DTO.
        /// </summary>
        /// <returns>The DTO.</returns>
        public TodoItemDto ToDto()
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
    }

    /// <summary>
    /// The <see cref="TodoItemSchedule"/> extensions.
    /// </summary>
    /// <param name="valueObject">The value object.</param>
    extension(TodoItemSchedule valueObject)
    {
        private TodoItemScheduleDto ToDto()
        {
            return new TodoItemScheduleDto(valueObject.DueDate);
        }
    }
}