using Architecture.Tests.Extensions;
using Architecture.Tests.Helpers;
using NetArchTest.Rules;
using SharedCore.Presentation.Endpoints;

namespace Architecture.Tests;

public class PresentationTests
{
    [Fact]
    public void Presentation_ShouldNotBeReferencedByAnyAssembly()
    {
        // Arrange
        var conditionList = Types.InAssemblies(Assemblies.GetAllExcept(Assemblies.Presentation))
            .ShouldNot().HaveDependencyOnAny(Namespaces.Presentation);

        // Act
        var result = conditionList.GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Presentation_EndpointGroups_ShouldBeInPresentationAssembly()
    {
        // Arrange
        var conditionList = Types.InAssemblies(Assemblies.All)
            .That().ImplementInterface(typeof(IEndpointGroup))
            .Should().ResideInAnyOfNamespaces(Namespaces.Presentation);

        // Act
        var result = conditionList.GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Presentation_EndpointGroups_ShouldHaveCorrectName()
    {
        // Arrange
        var conditionList = Types.InAssemblies(Assemblies.Presentation)
            .That().ImplementInterface(typeof(IEndpointGroup))
            .Should().HaveNameEndingWith(ClassNames.EndpointGroupNameEnding);

        // Act
        var result = conditionList.GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Application_EndpointGroups_ShouldHaveCorrectNamespace()
    {
        // Arrange
        var conditionList = Types.InAssemblies(Assemblies.Presentation)
            .That().ImplementInterface(typeof(IEndpointGroup))
            .Should().ResideInNamespaceContaining(Namespaces.PatternForEndpointGroup);

        // Act
        var result = conditionList.GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }
}