using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using SharedCore.Common.ApplicationContext;
using SharedCore.Common.Authorization;

namespace SharedCore.Common.Tests.ApplicationContext;

public class CurrentUserTests
{
    private readonly ICurrentUser _currentUser;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserTests()
    {
        _httpContextAccessor = Substitute.For<IHttpContextAccessor>();

        _currentUser = new CurrentUser(_httpContextAccessor);
    }

    [Fact]
    public void IsAuthenticated_WhenUserIsAuthenticated_ShouldReturnTrue()
    {
        // Arrange
        _httpContextAccessor.HttpContext.User.Identity!.IsAuthenticated.Returns(true);

        // Act
        var result = _currentUser.IsAuthenticated;

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsAuthenticated_WhenUserIsNotAuthenticated_ShouldReturnFalse()
    {
        // Arrange
        _httpContextAccessor.HttpContext.User.Identity!.IsAuthenticated.Returns(false);

        // Act
        var result = _currentUser.IsAuthenticated;

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsAuthenticated_WhenIdentityIsNull_ShouldReturnFalse()
    {
        // Arrange
        _httpContextAccessor.HttpContext.User.Identity.Returns((IIdentity?)null);

        // Act
        var result = _currentUser.IsAuthenticated;

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsAuthenticated_WhenHttpContextIsNull_ShouldReturnFalse()
    {
        // Arrange
        _httpContextAccessor.HttpContext.Returns((HttpContext?)null);

        // Act
        var result = _currentUser.IsAuthenticated;

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsAuthenticated_WhenHttpContextAccessorIsNull_ShouldReturnFalse()
    {
        // Arrange
        IHttpContextAccessor? httpContextAccessor = null;
        var currentUser = new CurrentUser(httpContextAccessor);

        // Act
        var result = currentUser.IsAuthenticated;

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void UserId_WhenUserIsAuthenticated_ShouldReturnUserId()
    {
        // Arrange
        const string userId = "userId";
        _httpContextAccessor.HttpContext.User.FindFirst(JwtClaims.Subject).Returns(new Claim(JwtClaims.Subject, userId));

        // Act
        var result = _currentUser.UserId;

        // Assert
        Assert.Equal(userId, result);
    }

    [Fact]
    public void UserId_WhenUserIsNotAuthenticated_ShouldReturnNull()
    {
        // Arrange
        _httpContextAccessor.HttpContext.User.Claims.Returns([]);

        // Act
        var result = _currentUser.UserId;

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void UserId_WhenHttpContextIsNull_ShouldReturnNull()
    {
        // Arrange
        _httpContextAccessor.HttpContext.Returns((HttpContext?)null);

        // Act
        var result = _currentUser.UserId;

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void UserId_WhenHttpContextAccessorIsNull_ShouldReturnNull()
    {
        // Arrange
        IHttpContextAccessor? httpContextAccessor = null;
        var currentUser = new CurrentUser(httpContextAccessor);

        // Act
        var result = currentUser.UserId;

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void IsInRole_WhenUserIsInRole_ShouldReturnTrue()
    {
        // Arrange
        const string roleName = "Admin";
        _httpContextAccessor.HttpContext.User.IsInRole(roleName).Returns(true);

        // Act
        var result = _currentUser.IsInRole(roleName);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsInRole_WhenUserIsNotInRole_ShouldReturnFalse()
    {
        // Arrange
        const string roleName = "Admin";
        _httpContextAccessor.HttpContext.User.IsInRole(roleName).Returns(false);

        // Act
        var result = _currentUser.IsInRole(roleName);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsInRole_WhenHttpContextIsNull_ShouldReturnFalse()
    {
        // Arrange
        _httpContextAccessor.HttpContext.Returns((HttpContext?)null);

        // Act
        var result = _currentUser.IsInRole("roleName");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsInRole_WhenHttpContextAccessorIsNull_ShouldReturnFalse()
    {
        // Arrange
        IHttpContextAccessor? httpContextAccessor = null;
        var currentUser = new CurrentUser(httpContextAccessor);

        // Act
        var result = currentUser.IsInRole("roleName");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HasClaim_WhenUserHasClaim_ShouldReturnTrue()
    {
        // Arrange
        const string claimType = "claimType";
        const string claimValue = "claimValue";
        _httpContextAccessor.HttpContext.User.HasClaim(claimType, claimValue).Returns(true);

        // Act
        var result = _currentUser.HasClaim(claimType, claimValue);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasClaim_WhenUserDoesNotHaveClaim_ShouldReturnFalse()
    {
        // Arrange
        const string claimType = "claimType";
        const string claimValue = "claimValue";
        _httpContextAccessor.HttpContext.User.HasClaim(claimType, claimValue).Returns(false);

        // Act
        var result = _currentUser.HasClaim(claimType, claimValue);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HasClaim_WhenHttpContextIsNull_ShouldReturnFalse()
    {
        // Arrange
        _httpContextAccessor.HttpContext.Returns((HttpContext?)null);

        // Act
        var result = _currentUser.HasClaim("claimType", "claimValue");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HasClaim_WhenHttpContextAccessorIsNull_ShouldReturnFalse()
    {
        // Arrange
        IHttpContextAccessor? httpContextAccessor = null;
        var currentUser = new CurrentUser(httpContextAccessor);

        // Act
        var result = currentUser.HasClaim("claimType", "claimValue");

        // Assert
        Assert.False(result);
    }
}