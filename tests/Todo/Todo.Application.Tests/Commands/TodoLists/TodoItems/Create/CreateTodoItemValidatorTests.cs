using FluentValidation.TestHelper;
using Todo.Application.Commands.TodoLists.TodoItems.Create;
using Todo.Application.Dtos.TodoLists.TodoItems;
using Todo.Application.Dtos.TodoLists.TodoItems.Create;

namespace Todo.Application.Tests.Commands.TodoLists.TodoItems.Create;

public class CreateTodoItemValidatorTests
{
    private const int MaxLengthForTitle = 64;
    private const int MaxLengthForDescription = 512;

    private readonly CreateTodoItemValidator _validator;

    public CreateTodoItemValidatorTests()
    {
        _validator = new CreateTodoItemValidator();
    }

    [Theory]
    [MemberData(nameof(InvalidData))]
    public async Task ValidateAsync_WhenInvalidData_ShouldReturnError(CreateTodoItemDto invalidInput,
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

    [Theory]
    [MemberData(nameof(ValidData))]
    public async Task ValidateAsync_WhenValidData_ShouldSucceed(CreateTodoItemDto validInput)
    {
        // Arrange

        // Act
        var result =
            await _validator.TestValidateAsync(validInput, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    public static TheoryData<CreateTodoItemDto, string[]> InvalidData =>
        new()
        {
            {
                new CreateTodoItemDto { Title = null! },
                [nameof(CreateTodoItemDto.ListId), nameof(CreateTodoItemDto.Title)]
            },
            {
                new CreateTodoItemDto { ListId = Guid.Empty, Title = string.Empty },
                [nameof(CreateTodoItemDto.ListId), nameof(CreateTodoItemDto.Title)]
            },
            { new CreateTodoItemDto { ListId = Guid.NewGuid(), Title = " " }, [nameof(CreateTodoItemDto.Title)] },
            {
                new CreateTodoItemDto { Title = new string('A', MaxLengthForTitle + 1) },
                [nameof(CreateTodoItemDto.ListId), nameof(CreateTodoItemDto.Title)]
            },
            {
                new CreateTodoItemDto { Title = "Title", Description = new string('A', MaxLengthForDescription + 1) },
                [nameof(CreateTodoItemDto.ListId), nameof(CreateTodoItemDto.Description)]
            },
            {
                new CreateTodoItemDto
                {
                    Title = "Title", Schedule = new TodoItemScheduleDto(DateTimeOffset.UtcNow.AddSeconds(-1))
                },
                [
                    nameof(CreateTodoItemDto.ListId),
                    $"{nameof(CreateTodoItemDto.Schedule)}.{nameof(CreateTodoItemDto.Schedule.DueDate)}"
                ]
            }
        };

    public static TheoryData<CreateTodoItemDto> ValidData =>
    [
        new CreateTodoItemDto
        {
            ListId = Guid.NewGuid(),
            Title = "Title",
            Description = null,
            Schedule = new TodoItemScheduleDto(DateTimeOffset.UtcNow.AddDays(1))
        },
        new CreateTodoItemDto
        {
            ListId = Guid.NewGuid(),
            Title = "Title",
            Description = "Description",
            Schedule = new TodoItemScheduleDto(DateTimeOffset.UtcNow.AddDays(1))
        }
    ];
}