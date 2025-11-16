using System.Diagnostics.CodeAnalysis;
using Todo.Application.Dtos.TodoLists;
using Todo.Application.Dtos.TodoLists.Create;
using Todo.Application.Dtos.TodoLists.Update;
using Todo.Presentation.Api.Dtos.V1.TodoLists;
using Todo.Presentation.Api.Dtos.V1.TodoLists.Create;
using Todo.Presentation.Api.Dtos.V1.TodoLists.Update;

namespace Todo.Presentation.Api.MappingExtensions.V1.TodoLists;

/// <summary>
/// The to do list mapping extensions.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class TodoListMappingExtensions
{
    /// <summary>
    /// Converts the application DTO into an API DTO.
    /// </summary>
    /// <param name="dto">The application DTO.</param>
    /// <returns>The API DTO.</returns>
    public static TodoListApiDto ToApiDto(this TodoListDto dto)
    {
        return new TodoListApiDto
        {
            Id = dto.Id,
            Name = dto.Name,
            Items = dto.Items?.Select(i => i.ToApiDto()).ToList().AsReadOnly()
        };
    }

    /// <summary>
    /// Converts the API DTO into an application DTO.
    /// </summary>
    /// <param name="apiDto">The API DTO.</param>
    /// <returns>The application DTO.</returns>
    public static CreateTodoListDto ToDto(this CreateTodoListApiDto apiDto)
    {
        return new CreateTodoListDto
        {
            Name = apiDto.Name
        };
    }

    /// <summary>
    /// Converts the API DTO into an application DTO.
    /// </summary>
    /// <param name="apiDto">The API DTO.</param>
    /// <param name="id">The to do list identifier.</param>
    /// <returns>The application DTO.</returns>
    public static UpdateTodoListDto ToDto(this UpdateTodoListApiDto apiDto, Guid id)
    {
        return new UpdateTodoListDto
        {
            Id = id,
            Name = apiDto.Name
        };
    }
}