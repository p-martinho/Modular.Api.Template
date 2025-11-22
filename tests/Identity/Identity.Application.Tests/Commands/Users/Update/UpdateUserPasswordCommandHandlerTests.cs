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

public class UpdateUserPasswordCommandHandlerTests
{
    private readonly UpdateUserPasswordCommandHandler _commandHandler;
    private readonly ICurrentUser _currentUser;
    private readonly UserManager<AppIdentityUser> _userManager;

    public UpdateUserPasswordCommandHandlerTests()
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

        _commandHandler = new UpdateUserPasswordCommandHandler(NullLogger<UpdateUserPasswordCommandHandler>.Instance,
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
        var commandIn = new UpdateUserPasswordDto
        {
            Id = userId,
            OldPassword = "OldPassword",
            NewPassword = "NewPassword"
        };
        _userManager.ChangePasswordAsync(user, commandIn.OldPassword, commandIn.NewPassword)
            .Returns(IdentityResult.Success);

        // Act
        var result = await _commandHandler.HandleAsync(commandIn, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.Success, result.Result.ResultType);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task HandleAsync_WhenNoPermissions_ShouldReturnNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        _currentUser.UserId.Returns("otherUserId");
        var commandIn = new UpdateUserPasswordDto
        {
            Id = userId,
            OldPassword = "OldPassword",
            NewPassword = "NewPassword"
        };

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
        var commandIn = new UpdateUserPasswordDto
        {
            Id = userId,
            OldPassword = "OldPassword",
            NewPassword = "NewPassword"
        };

        // Act
        var result = await _commandHandler.HandleAsync(commandIn, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.NotFoundError, result.Result.ResultType);
        Assert.Null(result.Data);
        await _userManager.Received(1).FindByIdAsync(userId);
    }

    [Fact]
    public async Task HandleAsync_WhenErrorUpdatingPassword_ShouldReturnValidationError()
    {
        // Arrange
        var user = AppIdentityUser.Create("Email", "FirstName", "LastName")!;
        var userId = user.Id;
        _currentUser.UserId.Returns(userId);
        _userManager.FindByIdAsync(userId).Returns(user);
        var commandIn = new UpdateUserPasswordDto
        {
            Id = userId,
            OldPassword = "OldPassword",
            NewPassword = "NewPassword"
        };
        IdentityError[] errors =
        [
            new() { Code = "code1", Description = "description1" },
            new() { Code = "code2", Description = "description2" }
        ];
        _userManager.ChangePasswordAsync(null!, null!, null!).ReturnsForAnyArgs(IdentityResult.Failed(errors));

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