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
    /// The <see cref="TodoListDto"/> extensions.
    /// </summary>
    /// <param name="dto">The application DTO.</param>
    extension(TodoListDto dto)
    {
        /// <summary>
        /// Converts the application DTO into an API DTO.
        /// </summary>
        /// <returns>The API DTO.</returns>
        public TodoListApiDto ToApiDto()
        {
            return new TodoListApiDto
            {
                Id = dto.Id,
                Name = dto.Name,
                Items = dto.Items?.Select(i => i.ToApiDto()).ToList().AsReadOnly()
            };
        }
    }

    /// <summary>
    /// The <see cref="CreateTodoListApiDto"/> extensions.
    /// </summary>
    /// <param name="apiDto">The API DTO.</param>
    extension(CreateTodoListApiDto apiDto)
    {
        /// <summary>
        /// Converts the API DTO into an application DTO.
        /// </summary>
        /// <returns>The application DTO.</returns>
        public CreateTodoListDto ToDto()
        {
            return new CreateTodoListDto
            {
                Name = apiDto.Name
            };
        }
    }

    /// <summary>
    /// The <see cref="UpdateTodoListApiDto"/> extensions.
    /// </summary>
    /// <param name="apiDto">The API DTO.</param>
    extension(UpdateTodoListApiDto apiDto)
    {
        /// <summary>
        /// Converts the API DTO into an application DTO.
        /// </summary>
        /// <param name="id">The to do list identifier.</param>
        /// <returns>The application DTO.</returns>
        public UpdateTodoListDto ToDto(Guid id)
        {
            return new UpdateTodoListDto
            {
                Id = id,
                Name = apiDto.Name
            };
        }
    }
}