using FluentValidation.TestHelper;
using Identity.Application.Commands.Users.Create;
using Identity.Application.Dtos.Users.Create;

namespace Identity.Application.Tests.Commands.Users.Create;

public class CreateUserValidatorTests
{
    private const int MaxLengthForName = 64;

    private readonly CreateUserValidator _validator;

    public CreateUserValidatorTests()
    {
        _validator = new CreateUserValidator();
    }

    [Theory]
    [MemberData(nameof(InvalidData))]
    public async Task ValidateAsync_WhenInvalidData_ShouldReturnError(CreateUserDto invalidInput,
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
    public async Task ValidateAsync_WhenValidData_ShouldSucceed(CreateUserDto validInput)
    {
        // Arrange

        // Act
        var result =
            await _validator.TestValidateAsync(validInput, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    public static TheoryData<CreateUserDto, string[]> InvalidData =>
        new()
        {
            {
                new CreateUserDto { Email = null!, Password = null! },
                [nameof(CreateUserDto.Email), nameof(CreateUserDto.Password)]
            },
            {
                new CreateUserDto { Email = "", Password = "" },
                [nameof(CreateUserDto.Email), nameof(CreateUserDto.Email), nameof(CreateUserDto.Password)]
            },
            {
                new CreateUserDto { Email = " ", Password = " " },
                [nameof(CreateUserDto.Email), nameof(CreateUserDto.Email), nameof(CreateUserDto.Password)]
            },
            {
                new CreateUserDto { Email = "invalidEmailAddress", Password = "password" },
                [nameof(CreateUserDto.Email)]
            },
            {
                new CreateUserDto
                {
                    Email = null!,
                    Password = "password",
                    FirstName = new string('A', MaxLengthForName + 1),
                    LastName = new string('A', MaxLengthForName + 1)
                },
                [nameof(CreateUserDto.Email), nameof(CreateUserDto.FirstName), nameof(CreateUserDto.LastName)]
            }
        };

    public static TheoryData<CreateUserDto> ValidData =>
    [
        new CreateUserDto
        {
            Email = "email@email.com", Password = "Password", FirstName = "FirstName", LastName = "LastName"
        }
    ];
}