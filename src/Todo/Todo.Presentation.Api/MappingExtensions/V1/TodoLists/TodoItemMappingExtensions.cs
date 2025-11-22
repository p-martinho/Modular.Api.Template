using System.Diagnostics.CodeAnalysis;
using Todo.Application.Dtos.TodoLists.TodoItems;
using Todo.Application.Dtos.TodoLists.TodoItems.Create;
using Todo.Application.Dtos.TodoLists.TodoItems.Update;
using Todo.Presentation.Api.Dtos.V1.TodoLists.TodoItems;
using Todo.Presentation.Api.Dtos.V1.TodoLists.TodoItems.Create;
using Todo.Presentation.Api.Dtos.V1.TodoLists.TodoItems.Update;

namespace Todo.Presentation.Api.MappingExtensions.V1.TodoLists;

/// <summary>
/// The to do item mapping extensions.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class TodoItemMappingExtensions
{
    /// <summary>
    /// The <see cref="TodoItemDto"/> extensions.
    /// </summary>
    /// <param name="dto">The application DTO.</param>
    extension(TodoItemDto dto)
    {
        /// <summary>
        /// Converts the application DTO into an API DTO.
        /// </summary>
        /// <returns>The API DTO.</returns>
        public TodoItemApiDto ToApiDto()
        {
            return new TodoItemApiDto
            {
                Id = dto.Id,
                Title = dto.Title,
                Description = dto.Description,
                IsDone = dto.IsDone,
                Schedule = dto.Schedule.ToApiDto()
            };
        }
    }

    /// <summary>
    /// The <see cref="CreateTodoItemApiDto"/> extensions.
    /// </summary>
    /// <param name="apiDto">The API DTO.</param>
    extension(CreateTodoItemApiDto apiDto)
    {
        /// <summary>
        /// Converts the API DTO into an application DTO.
        /// </summary>
        /// <param name="todoListId">The to do list identifier.</param>
        /// <returns>The application DTO.</returns>
        public CreateTodoItemDto ToDto(Guid todoListId)
        {
            return new CreateTodoItemDto
            {
                ListId = todoListId,
                Title = apiDto.Title,
                Description = apiDto.Description,
                Schedule = apiDto.Schedule.ToDto()
            };
        }
    }

    /// <summary>
    /// The <see cref="UpdateTodoItemApiDto"/> extensions.
    /// </summary>
    /// <param name="apiDto">The API DTO.</param>
    extension(UpdateTodoItemApiDto apiDto)
    {
        /// <summary>
        /// Converts the API DTO into an application DTO.
        /// </summary>
        /// <param name="todoListId">The to do list identifier.</param>
        /// <param name="todoItemId">The to do item identifier.</param>
        /// <returns>The application DTO.</returns>
        public UpdateTodoItemDto ToDto(Guid todoListId, Guid todoItemId)
        {
            return new UpdateTodoItemDto
            {
                ListId = todoListId,
                Id = todoItemId,
                Title = apiDto.Title,
                Description = apiDto.Description,
                IsDone = apiDto.IsDone,
                Schedule = apiDto.Schedule.ToDto()
            };
        }
    }

    /// <summary>
    /// The <see cref="TodoItemScheduleDto"/> extensions.
    /// </summary>
    /// <param name="dto">The application DTO.</param>
    extension(TodoItemScheduleDto dto)
    {
        private TodoItemScheduleApiDto ToApiDto()
        {
            return new TodoItemScheduleApiDto(dto.DueDate);
        }
    }

    /// <summary>
    /// The <see cref="TodoItemScheduleApiDto"/> extensions.
    /// </summary>
    /// <param name="apiDto">The API DTO.</param>
    extension(TodoItemScheduleApiDto? apiDto)
    {
        private TodoItemScheduleDto? ToDto()
        {
            return apiDto is null ? null : new TodoItemScheduleDto(apiDto.DueDate);
        }
    }
}