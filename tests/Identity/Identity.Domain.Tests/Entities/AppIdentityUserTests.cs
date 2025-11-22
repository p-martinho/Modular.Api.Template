using Identity.Domain.Entities.Users;

namespace Identity.Domain.Tests.Entities;

public class AppIdentityUserTests
{
    [Fact]
    public void Create_ShouldSucceed()
    {
        // Arrange
        const string email = "Email";
        const string firstName = "FirstName";
        const string lastName = "LastName";

        // Act
        var result = AppIdentityUser.Create(email, firstName, lastName);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(email, result.Email);
        Assert.NotNull(result.Name);
        Assert.Equal(firstName, result.Name.FirstName);
        Assert.Equal(lastName, result.Name.LastName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_WhenInvalidInput_ShouldReturnNull(string? email)
    {
        // Arrange

        // Act
        var result = AppIdentityUser.Create(email!, null, null);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void UpdateName_ShouldSucceed()
    {
        // Arrange
        const string newFirstName = "newFirstName";
        const string newLastName = "newLastName";
        var user = AppIdentityUser.Create("email", "firstName", "lastName")!;

        // Act
        var result = user.UpdateName(newFirstName, newLastName);

        // Assert
        Assert.True(result);
        Assert.Equal(newFirstName, user.Name.FirstName);
        Assert.Equal(newLastName, user.Name.LastName);
    }
}