namespace Todo.Presentation.Api.Dtos.V1.TodoLists.TodoItems;

/// <summary>
/// The to do item schedule API DTO.
/// </summary>
/// <param name="DueDate">The due date.</param>
public record TodoItemScheduleApiDto(DateTimeOffset? DueDate);