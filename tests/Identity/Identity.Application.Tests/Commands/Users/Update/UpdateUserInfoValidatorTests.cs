using FluentValidation.TestHelper;
using Identity.Application.Commands.Users.Update;
using Identity.Application.Dtos.Users.Update;

namespace Identity.Application.Tests.Commands.Users.Update;

public class UpdateUserInfoValidatorTests
{
    private const int MaxLengthForName = 64;

    private readonly UpdateUserInfoValidator _validator;

    public UpdateUserInfoValidatorTests()
    {
        _validator = new UpdateUserInfoValidator();
    }

    [Theory]
    [MemberData(nameof(InvalidData))]
    public async Task ValidateAsync_WhenInvalidData_ShouldReturnError(UpdateUserInfoDto invalidInput,
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
    public async Task ValidateAsync_WhenValidData_ShouldSucceed(UpdateUserInfoDto validInput)
    {
        // Arrange

        // Act
        var result =
            await _validator.TestValidateAsync(validInput, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    public static TheoryData<UpdateUserInfoDto, string[]> InvalidData =>
        new()
        {
            { new UpdateUserInfoDto { Id = null! }, [nameof(UpdateUserInfoDto.Id)] },
            {
                new UpdateUserInfoDto { Id = "", Email = "" },
                [nameof(UpdateUserInfoDto.Id), nameof(UpdateUserInfoDto.Email)]
            },
            {
                new UpdateUserInfoDto { Id = " ", Email = " " },
                [nameof(UpdateUserInfoDto.Id), nameof(UpdateUserInfoDto.Email)]
            },
            {
                new UpdateUserInfoDto { Id = "Id", Email = "invalidEmailAddress" },
                [nameof(UpdateUserInfoDto.Email)]
            },
            {
                new UpdateUserInfoDto
                {
                    Id = "Id",
                    FirstName = new string('A', MaxLengthForName + 1),
                    LastName = new string('A', MaxLengthForName + 1)
                },
                [nameof(UpdateUserInfoDto.FirstName), nameof(UpdateUserInfoDto.LastName)]
            }
        };

    public static TheoryData<UpdateUserInfoDto> ValidData =>
    [
        new UpdateUserInfoDto
        {
            Id = "Id",
            Email = "email@email.com",
            FirstName = "FirstName",
            LastName = "LastName"
        }
    ];
}