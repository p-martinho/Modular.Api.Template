using FluentValidation.TestHelper;
using Identity.Application.Commands.Users.Update;
using Identity.Application.Dtos.Users.Update;

namespace Identity.Application.Tests.Commands.Users.Update;

public class UpdateUserPasswordValidatorTests
{
    private readonly UpdateUserPasswordValidator _validator;

    public UpdateUserPasswordValidatorTests()
    {
        _validator = new UpdateUserPasswordValidator();
    }

    [Theory]
    [MemberData(nameof(InvalidData))]
    public async Task ValidateAsync_WhenInvalidData_ShouldReturnError(UpdateUserPasswordDto invalidInput,
        string[] fieldsThatShouldFail)
    {
        // Arrange

        // Act
        var result = await _validator
            .TestValidateAsync(invalidInput, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        foreach (var field in fieldsThatShouldFail)
        {
            result.ShouldHaveValidationErrorFor(field);
        }

        Assert.Equal(fieldsThatShouldFail.Length, result.Errors.Count);
    }

    [Theory]
    [MemberData(nameof(ValidData))]
    public async Task ValidateAsync_WhenValidData_ShouldSucceed(UpdateUserPasswordDto validInput)
    {
        // Arrange

        // Act
        var result =
            await _validator.TestValidateAsync(validInput, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    public static TheoryData<UpdateUserPasswordDto, string[]> InvalidData =>
        new()
        {
            {
                new UpdateUserPasswordDto { Id = null!, OldPassword = null!, NewPassword = null! }, [
                    nameof(UpdateUserPasswordDto.Id), nameof(UpdateUserPasswordDto.OldPassword),
                    nameof(UpdateUserPasswordDto.NewPassword)
                ]
            },
            {
                new UpdateUserPasswordDto { Id = "", OldPassword = "", NewPassword = "" }, [
                    nameof(UpdateUserPasswordDto.Id), nameof(UpdateUserPasswordDto.OldPassword),
                    nameof(UpdateUserPasswordDto.NewPassword)
                ]
            },
            {
                new UpdateUserPasswordDto { Id = " ", OldPassword = " ", NewPassword = " " }, [
                    nameof(UpdateUserPasswordDto.Id), nameof(UpdateUserPasswordDto.OldPassword),
                    nameof(UpdateUserPasswordDto.NewPassword)
                ]
            }
        };

    public static TheoryData<UpdateUserPasswordDto> ValidData =>
    [
        new UpdateUserPasswordDto { Id = "Id", OldPassword = "OldPassword", NewPassword = "NewPassword" }
    ];
}