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
    /// Converts the application DTO into an API DTO.
    /// </summary>
    /// <param name="dto">The application DTO.</param>
    /// <returns>The API DTO.</returns>
    public static TodoItemApiDto ToApiDto(this TodoItemDto dto)
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

    /// <summary>
    /// Converts the API DTO into an application DTO.
    /// </summary>
    /// <param name="apiDto">The API DTO.</param>
    /// <param name="todoListId">The to do list identifier.</param>
    /// <returns>The application DTO.</returns>
    public static CreateTodoItemDto ToDto(this CreateTodoItemApiDto apiDto, Guid todoListId)
    {
        return new CreateTodoItemDto
        {
            ListId = todoListId,
            Title = apiDto.Title,
            Description = apiDto.Description,
            Schedule = apiDto.Schedule.ToDto()
        };
    }

    /// <summary>
    /// Converts the API DTO into an application DTO.
    /// </summary>
    /// <param name="apiDto">The API DTO.</param>
    /// <param name="todoListId">The to do list identifier.</param>
    /// <param name="todoItemId">The to do item identifier.</param>
    /// <returns>The application DTO.</returns>
    public static UpdateTodoItemDto ToDto(this UpdateTodoItemApiDto apiDto, Guid todoListId, Guid todoItemId)
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

    private static TodoItemScheduleApiDto ToApiDto(this TodoItemScheduleDto dto)
    {
        return new TodoItemScheduleApiDto(dto.DueDate);
    }

    private static TodoItemScheduleDto? ToDto(this TodoItemScheduleApiDto? apiDto)
    {
        return apiDto is null ? null : new TodoItemScheduleDto(apiDto.DueDate);
    }
}