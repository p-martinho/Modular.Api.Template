using FluentValidation.TestHelper;
using Todo.Application.Commands.TodoLists.Update;
using Todo.Application.Dtos.TodoLists.Update;

namespace Todo.Application.Tests.Commands.TodoLists.Update;

public class UpdateTodoListValidatorTests
{
    private const int MaxLengthForName = 64;

    private readonly UpdateTodoListValidator _validator;

    public UpdateTodoListValidatorTests()
    {
        _validator = new UpdateTodoListValidator();
    }

    [Theory]
    [MemberData(nameof(InvalidData))]
    public async Task ValidateAsync_WhenInvalidData_ShouldReturnError(UpdateTodoListDto invalidInput,
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
    public async Task ValidateAsync_WhenValidData_ShouldSucceed(UpdateTodoListDto validInput)
    {
        // Arrange

        // Act
        var result = await _validator.TestValidateAsync(validInput, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    public static TheoryData<UpdateTodoListDto, string[]> InvalidData =>
        new()
        {
            {
                new UpdateTodoListDto { Name = string.Empty },
                [nameof(UpdateTodoListDto.Id), nameof(UpdateTodoListDto.Name)]
            },
            {
                new UpdateTodoListDto { Name = " " },
                [nameof(UpdateTodoListDto.Id), nameof(UpdateTodoListDto.Name)]
            },
            {
                new UpdateTodoListDto { Id = Guid.NewGuid(), Name = new string('A', MaxLengthForName + 1) },
                [nameof(UpdateTodoListDto.Name)]
            }
        };

    public static TheoryData<UpdateTodoListDto> ValidData =>
    [
        new UpdateTodoListDto { Id = Guid.NewGuid(), Name = null },
        new UpdateTodoListDto { Id = Guid.NewGuid(), Name = "Name" }
    ];
}