using NetArchTest.Rules;

namespace Architecture.Tests;

public class GenericTests
{
    [Fact]
    public void Interface_ShouldHaveNameStartingWithI()
    {
        // Arrange
        var conditionList = Types.InCurrentDomain()
            .That().AreInterfaces()
            .Should().HaveNameStartingWith("I");

        // Act
        var result = conditionList.GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }
}