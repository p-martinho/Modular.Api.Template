namespace Todo.Application.Dtos.TodoLists.TodoItems;

/// <summary>
/// The to do item schedule DTO.
/// </summary>
/// <param name="DueDate">The due date.</param>
public record TodoItemScheduleDto(DateTimeOffset? DueDate);