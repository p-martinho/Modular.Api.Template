using FluentValidation.TestHelper;
using Todo.Application.Commands.TodoLists.Create;
using Todo.Application.Dtos.TodoLists.Create;

namespace Todo.Application.Tests.Commands.TodoLists.Create;

public class CreateTodoListValidatorTests
{
    private const int MaxLengthForName = 64;

    private readonly CreateTodoListValidator _validator;

    public CreateTodoListValidatorTests()
    {
        _validator = new CreateTodoListValidator();
    }

    [Theory]
    [MemberData(nameof(InvalidData))]
    public async Task ValidateAsync_WhenInvalidData_ShouldReturnError(CreateTodoListDto invalidInput,
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
    public async Task ValidateAsync_WhenValidData_ShouldSucceed(CreateTodoListDto validInput)
    {
        // Arrange

        // Act
        var result =
            await _validator.TestValidateAsync(validInput, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    public static TheoryData<CreateTodoListDto, string[]> InvalidData =>
        new()
        {
            { new CreateTodoListDto { Name = null! }, [nameof(CreateTodoListDto.Name)] },
            { new CreateTodoListDto { Name = string.Empty }, [nameof(CreateTodoListDto.Name)] },
            { new CreateTodoListDto { Name = " " }, [nameof(CreateTodoListDto.Name)] },
            {
                new CreateTodoListDto { Name = new string('A', MaxLengthForName + 1) },
                [nameof(CreateTodoListDto.Name)]
            }
        };

    public static TheoryData<CreateTodoListDto> ValidData =>
    [
        new CreateTodoListDto { Name = "Name" }
    ];
}