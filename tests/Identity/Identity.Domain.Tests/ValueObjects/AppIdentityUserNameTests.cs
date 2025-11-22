using Identity.Domain.ValueObjects;

namespace Identity.Domain.Tests.ValueObjects;

public class AppIdentityUserNameTests
{
    [Fact]
    public void GetFullName_ShouldReturnFullName()
    {
        // Arrange
        const string firstName = "FirstName";
        const string lastName = "LastName";
        var name = new AppIdentityUserName(firstName, lastName);

        // Act
        var result = name.GetFullName();

        // Assert
        Assert.Equal($"{firstName} {lastName}", result);
    }

    [Theory]
    [InlineData(null, null, null)]
    [InlineData("", "", null)]
    [InlineData(" ", " ", null)]
    [InlineData("FirstName", null, "FirstName")]
    [InlineData("FirstName", "", "FirstName")]
    [InlineData("FirstName", " ", "FirstName")]
    [InlineData(null, "LastName", "LastName")]
    [InlineData("", "LastName", "LastName")]
    [InlineData(" ", "LastName", "LastName")]
    public void GetFullName_WhenFirstNameOrLastNameIsNullOrEmpty_ShouldReturnCorrectName(string? firstName,
        string? lastName, string? expectedResult)
    {
        // Arrange
        var name = new AppIdentityUserName(firstName, lastName);

        // Act
        var result = name.GetFullName();

        // Assert
        Assert.Equal(expectedResult, result);
    }
}