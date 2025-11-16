using Architecture.Tests.Helpers;
using NetArchTest.Rules;

namespace Architecture.Tests;

public class CommonTests
{
    [Fact]
    public void Common_ShouldOnlyHaveSpecificDependencies()
    {
        // Arrange
        var conditionList = Types.InAssemblies(Assemblies.Common)
            .Should().OnlyHaveDependencyOn([Namespaces.System, Namespaces.Microsoft, .. Namespaces.Common]);

        // Act
        var result = conditionList.GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }
}