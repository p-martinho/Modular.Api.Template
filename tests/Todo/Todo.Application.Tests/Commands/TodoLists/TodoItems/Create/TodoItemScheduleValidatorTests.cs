using FluentValidation.TestHelper;
using Todo.Application.Commands.TodoLists.TodoItems.Create;
using Todo.Application.Dtos.TodoLists.TodoItems;

namespace Todo.Application.Tests.Commands.TodoLists.TodoItems.Create;

public class TodoItemScheduleValidatorTests
{
    private readonly TodoItemScheduleValidator _validator;

    public TodoItemScheduleValidatorTests()
    {
        _validator = new TodoItemScheduleValidator();
    }

    [Theory]
    [MemberData(nameof(InvalidData))]
    public async Task ValidateAsync_WhenInvalidData_ShouldReturnError(TodoItemScheduleDto invalidInput,
        string[] fieldsThatShouldFail)
    {
        // Arrange

        // Act
        var result =
            await _validator.TestValidateAsync(invalidInput, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        foreach (var field in fieldsThatShouldFail)
        {
            result.ShouldHaveValidationErrorFor(field);
        }

        Assert.Equal(fieldsThatShouldFail.Length, result.Errors.Count);
    }

    [Fact]
    public async Task ValidateAsync_WhenValidData_ShouldSucceed()
    {
        // Arrange
        var validInput = new TodoItemScheduleDto(DateTimeOffset.UtcNow.AddDays(1));

        // Act
        var result =
            await _validator.TestValidateAsync(validInput, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    public static TheoryData<TodoItemScheduleDto, string[]> InvalidData =>
        new()
        {
            { new TodoItemScheduleDto(DateTimeOffset.UtcNow.AddSeconds(-1)), [nameof(TodoItemScheduleDto.DueDate)] }
        };
}