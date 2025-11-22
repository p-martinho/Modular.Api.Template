using Identity.Application.Queries.Users.GetById;
using Identity.Domain.Entities.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using SharedCore.Application.Common.Models;
using SharedCore.Common.ApplicationContext;

namespace Identity.Application.Tests.Queries.Users.BetById;

public class GetUserByIdQueryHandlerTests
{
    private readonly GetUserByIdQueryHandler _queryHandler;
    private readonly ICurrentUser _currentUser;
    private readonly UserManager<AppIdentityUser> _userManager;

    public GetUserByIdQueryHandlerTests()
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

        _queryHandler = new GetUserByIdQueryHandler(NullLogger<GetUserByIdQueryHandler>.Instance,
        _currentUser,
        _userManager);
    }

    [Fact]
    public async Task HandleAsync_ShouldSucceed()
    {
        // Arrange
        var user = AppIdentityUser.Create("email", "firstName", "lastName")!;
        var userId = user.Id;
        _currentUser.UserId.Returns(userId);
        _userManager.FindByIdAsync(userId).Returns(user);

        // Act
        var result = await _queryHandler.HandleAsync(userId, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.Success, result.Result.ResultType);
        Assert.NotNull(result.Data);
        Assert.Equal(user.Email, result.Data.Email);
        Assert.Equal(user.Name.FirstName, result.Data.FirstName);
        Assert.Equal(user.Name.LastName, result.Data.LastName);
        Assert.Equal(user.Name.GetFullName(), result.Data.FullName);
    }

    [Fact]
    public async Task HandleAsync_WhenNoPermissions_ShouldReturnNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        _currentUser.UserId.Returns("otherUserId");

        // Act
        var result = await _queryHandler.HandleAsync(userId, TestContext.Current.CancellationToken);

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

        // Act
        var result = await _queryHandler.HandleAsync(userId, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.NotFoundError, result.Result.ResultType);
        Assert.Null(result.Data);
        await _userManager.Received(1).FindByIdAsync(userId);
    }
}