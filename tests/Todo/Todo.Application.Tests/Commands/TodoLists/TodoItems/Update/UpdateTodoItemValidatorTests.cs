using FluentValidation.TestHelper;
using Todo.Application.Commands.TodoLists.TodoItems.Update;
using Todo.Application.Dtos.TodoLists.TodoItems;
using Todo.Application.Dtos.TodoLists.TodoItems.Update;

namespace Todo.Application.Tests.Commands.TodoLists.TodoItems.Update;

public class UpdateTodoItemValidatorTests
{
    private const int MaxLengthForTitle = 64;
    private const int MaxLengthForDescription = 512;

    private readonly UpdateTodoItemValidator _validator;

    public UpdateTodoItemValidatorTests()
    {
        _validator = new UpdateTodoItemValidator();
    }

    [Theory]
    [MemberData(nameof(InvalidData))]
    public async Task ValidateAsync_WhenInvalidData_ShouldReturnError(UpdateTodoItemDto invalidInput,
        string[] fieldsThatShouldFail)
    {
        // Arrange

        // Act
        var result = await _validator.TestValidateAsync(invalidInput, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        foreach (var field in fieldsThatShouldFail)
        {
            result.ShouldHaveValidationErrorFor(field);
        }

        Assert.Equal(fieldsThatShouldFail.Length, result.Errors.Count);
    }

    [Theory]
    [MemberData(nameof(ValidData))]
    public async Task ValidateAsync_WhenValidData_ShouldSucceed(UpdateTodoItemDto validInput)
    {
        // Arrange

        // Act
        var result = await _validator.TestValidateAsync(validInput, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    public static TheoryData<UpdateTodoItemDto, string[]> InvalidData =>
        new()
        {
            {
                new UpdateTodoItemDto { Title = string.Empty },
                [nameof(UpdateTodoItemDto.ListId), nameof(UpdateTodoItemDto.Id), nameof(UpdateTodoItemDto.Title)]
            },
            {
                new UpdateTodoItemDto { ListId = Guid.NewGuid(), Id = Guid.NewGuid(), Title = " " },
                [nameof(UpdateTodoItemDto.Title)]
            },
            {
                new UpdateTodoItemDto { Title = new string('A', MaxLengthForTitle + 1) },
                [nameof(UpdateTodoItemDto.ListId), nameof(UpdateTodoItemDto.Id), nameof(UpdateTodoItemDto.Title)]
            },
            {
                new UpdateTodoItemDto { Title = "Title", Description = new string('A', MaxLengthForDescription + 1) },
                [nameof(UpdateTodoItemDto.ListId), nameof(UpdateTodoItemDto.Id), nameof(UpdateTodoItemDto.Description)]
            },
            {
                new UpdateTodoItemDto
                {
                    Title = "Title",
                    Schedule = new TodoItemScheduleDto(DateTimeOffset.UtcNow.AddSeconds(-1))
                },
                [
                    nameof(UpdateTodoItemDto.ListId), nameof(UpdateTodoItemDto.Id),
                    $"{nameof(UpdateTodoItemDto.Schedule)}.{nameof(UpdateTodoItemDto.Schedule.DueDate)}"
                ]
            }
        };

    public static TheoryData<UpdateTodoItemDto> ValidData =>
    [
        new UpdateTodoItemDto { ListId = Guid.NewGuid(), Id = Guid.NewGuid() },
        new UpdateTodoItemDto { ListId = Guid.NewGuid(), Id = Guid.NewGuid(), Title = "Name" },
        new UpdateTodoItemDto { ListId = Guid.NewGuid(), Id = Guid.NewGuid(), Description = "Description" },
        new UpdateTodoItemDto
        {
            ListId = Guid.NewGuid(),
            Id = Guid.NewGuid(),
            Schedule = new TodoItemScheduleDto(DateTimeOffset.UtcNow.AddDays(1))
        },
        new UpdateTodoItemDto { ListId = Guid.NewGuid(), Id = Guid.NewGuid(), IsDone = false },
        new UpdateTodoItemDto { ListId = Guid.NewGuid(), Id = Guid.NewGuid(), IsDone = true }
    ];
}