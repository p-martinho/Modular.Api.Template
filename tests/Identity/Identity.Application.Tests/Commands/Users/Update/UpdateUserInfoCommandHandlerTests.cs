using Identity.Application.Commands.Users.Update;
using Identity.Application.Dtos.Users.Update;
using Identity.Domain.Entities.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using SharedCore.Application.Common.Models;
using SharedCore.Common.ApplicationContext;

namespace Identity.Application.Tests.Commands.Users.Update;

public class UpdateUserInfoCommandHandlerTests
{
    private readonly UpdateUserInfoCommandHandler _commandHandler;
    private readonly ICurrentUser _currentUser;
    private readonly UserManager<AppIdentityUser> _userManager;

    public UpdateUserInfoCommandHandlerTests()
    {
        _currentUser = Substitute.For<ICurrentUser>();

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

        _commandHandler = new UpdateUserInfoCommandHandler(NullLogger<UpdateUserInfoCommandHandler>.Instance,
            null!,
            _currentUser,
            _userManager);
    }

    [Fact]
    public async Task HandleAsync_ShouldSucceed()
    {
        // Arrange
        var user = AppIdentityUser.Create("Email", "FirstName", "LastName")!;
        var userId = user.Id;
        _currentUser.UserId.Returns(userId);
        _userManager.FindByIdAsync(userId).Returns(user);
        var commandIn = new UpdateUserInfoDto
        {
            Id = userId,
            Email = "NewEmail",
            FirstName = "NewFirstName",
            LastName = "NewLastName"
        };
        _userManager.ChangeEmailAsync(user, commandIn.Email, Arg.Any<string>())
            .Returns(IdentityResult.Success)
            .AndDoes(x => (x[0] as AppIdentityUser)!.Email = commandIn.Email);
        _userManager.SetUserNameAsync(user, commandIn.Email).Returns(IdentityResult.Success);
        _userManager.UpdateAsync(user).Returns(IdentityResult.Success);

        // Act
        var result = await _commandHandler.HandleAsync(commandIn, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.Success, result.Result.ResultType);
        Assert.NotNull(result.Data);
        Assert.Equal(commandIn.Email, user.Email);
        Assert.Equal(commandIn.FirstName, user.Name.FirstName);
        Assert.Equal(commandIn.LastName, user.Name.LastName);
        Assert.Equal(commandIn.Email, result.Data.Email);
        Assert.Equal(commandIn.FirstName, result.Data.FirstName);
        Assert.Equal(commandIn.LastName, result.Data.LastName);
    }

    [Fact]
    public async Task HandleAsync_WhenNothingToUpdate_ShouldNotUpdateAndSucceed()
    {
        // Arrange
        var user = AppIdentityUser.Create("Email", "FirstName", "LastName")!;
        var userId = user.Id;
        _currentUser.UserId.Returns(userId);
        _userManager.FindByIdAsync(userId).Returns(user);
        var commandIn = new UpdateUserInfoDto
        {
            Id = userId,
            Email = user.Email,
            FirstName = user.Name.FirstName,
            LastName = user.Name.LastName
        };

        // Act
        var result = await _commandHandler.HandleAsync(commandIn, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.Success, result.Result.ResultType);
        await _userManager.DidNotReceiveWithAnyArgs().ChangeEmailAsync(null!, null!, null!);
        await _userManager.DidNotReceiveWithAnyArgs().SetUserNameAsync(null!, null);
        await _userManager.DidNotReceiveWithAnyArgs().UpdateAsync(null!);
    }

    [Fact]
    public async Task HandleAsync_WhenAllInputsAreNull_ShouldNotUpdateAndSucceed()
    {
        // Arrange
        var user = AppIdentityUser.Create("Email", "FirstName", "LastName")!;
        var userId = user.Id;
        _currentUser.UserId.Returns(userId);
        _userManager.FindByIdAsync(userId).Returns(user);
        var commandIn = new UpdateUserInfoDto
        {
            Id = userId,
            Email = null,
            FirstName = null,
            LastName = null
        };

        // Act
        var result = await _commandHandler.HandleAsync(commandIn, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.Success, result.Result.ResultType);
        await _userManager.DidNotReceiveWithAnyArgs().ChangeEmailAsync(null!, null!, null!);
        await _userManager.DidNotReceiveWithAnyArgs().SetUserNameAsync(null!, null);
        await _userManager.DidNotReceiveWithAnyArgs().UpdateAsync(null!);
    }

    [Fact]
    public async Task HandleAsync_WhenNoPermissions_ShouldReturnNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        _currentUser.UserId.Returns("otherUserId");
        var commandIn = new UpdateUserInfoDto { Id = userId };

        // Act
        var result = await _commandHandler.HandleAsync(commandIn, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.NotFoundError, result.Result.ResultType);
        Assert.Null(result.Data);
        await _userManager.DidNotReceiveWithAnyArgs().FindByIdAsync(null!);
    }

    [Fact]
    public async Task HandleAsync_WhenNotFound_ShouldReturnNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        _currentUser.UserId.Returns(userId);
        _userManager.FindByIdAsync(userId).Returns((AppIdentityUser?)null);
        var commandIn = new UpdateUserInfoDto { Id = userId };

        // Act
        var result = await _commandHandler.HandleAsync(commandIn, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.NotFoundError, result.Result.ResultType);
        Assert.Null(result.Data);
        await _userManager.Received(1).FindByIdAsync(userId);
    }

    [Fact]
    public async Task HandleAsync_WhenErrorChangingEmail_ShouldReturnValidationError()
    {
        // Arrange
        var user = AppIdentityUser.Create("Email", "FirstName", "LastName")!;
        var userId = user.Id;
        _currentUser.UserId.Returns(userId);
        _userManager.FindByIdAsync(userId).Returns(user);
        var commandIn = new UpdateUserInfoDto { Id = userId, Email = "NewEmail" };
        IdentityError[] errors =
        [
            new() { Code = "code1", Description = "description1" },
            new() { Code = "code2", Description = "description2" }
        ];
        _userManager.ChangeEmailAsync(null!, null!, null!).ReturnsForAnyArgs(IdentityResult.Failed(errors));

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

    [Fact]
    public async Task HandleAsync_WhenErrorChangingUsername_ShouldReturnValidationError()
    {
        // Arrange
        var user = AppIdentityUser.Create("Email", "FirstName", "LastName")!;
        var userId = user.Id;
        _currentUser.UserId.Returns(userId);
        _userManager.FindByIdAsync(userId).Returns(user);
        var commandIn = new UpdateUserInfoDto { Id = userId, Email = "NewEmail" };
        IdentityError[] errors =
        [
            new() { Code = "code1", Description = "description1" },
            new() { Code = "code2", Description = "description2" }
        ];
        _userManager.ChangeEmailAsync(user, commandIn.Email, Arg.Any<string>())
            .Returns(IdentityResult.Success);
        _userManager.SetUserNameAsync(null!, null).ReturnsForAnyArgs(IdentityResult.Failed(errors));

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

    [Fact]
    public async Task HandleAsync_WhenErrorUpdatingName_ShouldReturnValidationError()
    {
        // Arrange
        var user = AppIdentityUser.Create("Email", "FirstName", "LastName")!;
        var userId = user.Id;
        _currentUser.UserId.Returns(userId);
        _userManager.FindByIdAsync(userId).Returns(user);
        var commandIn = new UpdateUserInfoDto { Id = userId, FirstName = "NewFirstName", LastName = "NewLastName" };
        IdentityError[] errors =
        [
            new() { Code = "code1", Description = "description1" },
            new() { Code = "code2", Description = "description2" }
        ];
        _userManager.UpdateAsync(null!).ReturnsForAnyArgs(IdentityResult.Failed(errors));

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