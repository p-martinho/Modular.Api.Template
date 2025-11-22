using Identity.Application.Commands.Users.Create;
using Identity.Application.Dtos.Users.Create;
using Identity.Domain.Entities.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using SharedCore.Application.Common.Models;

namespace Identity.Application.Tests.Commands.Users.Create;

public class CreateUserCommandHandlerTests
{
    private readonly CreateUserCommandHandler _commandHandler;
    private readonly UserManager<AppIdentityUser> _userManager;

    public CreateUserCommandHandlerTests()
    {
        _userManager = Substitute.For<UserManager<AppIdentityUser>>(
            Substitute.For<IUserStore<AppIdentityUser>>(),
            Substitute.For<IOptions<IdentityOptions>>(),
            Substitute.For<IPasswordHasher<AppIdentityUser>>(),
            Substitute.For<IEnumerable<IUserValidator<AppIdentityUser>>>(),
            Substitute.For<IEnumerable<IPasswordValidator<AppIdentityUser>>>(),
            Substitute.For<ILookupNormalizer>(),
            Substitute.For<IdentityErrorDescriber>(),
            Substitute.For<IServiceProvider>(),
            Substitute.For<ILogger<UserManager<AppIdentityUser>>>());

        var userStore = Substitute.For<IUserEmailStore<AppIdentityUser>>();

        _commandHandler = new CreateUserCommandHandler(NullLogger<CreateUserCommandHandler>.Instance,
            null!,
            _userManager,
            userStore);
    }

    [Fact]
    public async Task HandleAsync_ShouldSucceed()
    {
        // Arrange
        var commandIn = new CreateUserDto
        {
            Email = "Email",
            Password = "Password",
            FirstName = "FirstName",
            LastName = "LastName"
        };
        _userManager.CreateAsync(Arg.Is<AppIdentityUser>(u => u.Email == commandIn.Email), commandIn.Password)
            .Returns(IdentityResult.Success);

        // Act
        var result = await _commandHandler.HandleAsync(commandIn, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.Success, result.Result.ResultType);
        Assert.NotNull(result.Data);
        Assert.Equal(commandIn.Email, result.Data.Email);
        Assert.Equal(commandIn.FirstName, result.Data.FirstName);
        Assert.Equal(commandIn.LastName, result.Data.LastName);
        await _userManager.Received(1)
            .CreateAsync(Arg.Is<AppIdentityUser>(u => u.Email == commandIn.Email), commandIn.Password);
    }

    [Fact]
    public async Task HandleAsync_WhenInvalidInput_ShouldReturnValidationError()
    {
        // Arrange
        var commandIn = new CreateUserDto { Email = null!, Password = "Password" };

        // Act
        var result = await _commandHandler.HandleAsync(commandIn, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.ValidationError, result.Result.ResultType);
        Assert.Null(result.Data);
        await _userManager.DidNotReceiveWithAnyArgs().CreateAsync(null!, null!);
    }

    [Fact]
    public async Task HandleAsync_WhenErrorAddingUser_ShouldReturnValidationError()
    {
        // Arrange
        var commandIn = new CreateUserDto { Email = "Email", Password = "Password" };
        IdentityError[] errors =
        [
            new() { Code = "code1", Description = "description1" },
            new() { Code = "code2", Description = "description2" }
        ];
        _userManager.CreateAsync(null!, null!).ReturnsForAnyArgs(IdentityResult.Failed(errors));

        // Act
        var result = await _commandHandler.HandleAsync(commandIn, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.ValidationError, result.Result.ResultType);
        Assert.Null(result.Data);
        Assert.NotNull(result.Result.ResultDetails);
        Assert.Equal(errors.Length, result.Result.ResultDetails.Count());
        Assert.Equal(errors.First().Code, result.Result.ResultDetails.First().Property);
        Assert.Equal(errors.First().Description, result.Result.ResultDetails.First().Message);
        Assert.Equal(errors.Last().Code, result.Result.ResultDetails.Last().Property);
        Assert.Equal(errors.Last().Description, result.Result.ResultDetails.Last().Message);
    }
}