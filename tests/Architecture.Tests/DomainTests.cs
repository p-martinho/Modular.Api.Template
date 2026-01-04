using System.Reflection;
using Architecture.Tests.Helpers;
using Identity.Domain.Entities.Users;
using NetArchTest.Rules;
using SharedCore.Domain.Entities;

namespace Architecture.Tests;

public class DomainTests
{
    [Fact]
    public void Domain_ShouldOnlyHaveSpecificDependencies()
    {
        // Arrange
        var conditionList = Types.InAssemblies(Assemblies.Domain)
            .That().DoNotHaveName(nameof(AppIdentityUser)) // this is a special case
            .Should().OnlyHaveDependencyOn([Namespaces.System, Namespaces.Microsoft, .. Namespaces.Domain]);

        // Act
        var result = conditionList.GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Domain_ShouldNotBeReferencedBySpecificAssemblies()
    {
        // Arrange
        var conditionList = Types.InAssemblies([.. Assemblies.Presentation, .. Assemblies.Common])
            .ShouldNot().HaveDependencyOnAny(Namespaces.Domain);

        // Act
        var result = conditionList.GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Domain_Entities_ShouldNotHavePublicConstructors()
    {
        // Arrange
        var entityTypes = Types.InAssemblies(Assemblies.Domain)
            .That().Inherit<BaseEntity>()
            .GetTypes();

        // Act
        var failingTypes = new List<IType>();
        foreach (var entityType in entityTypes)
        {
            var constructors = entityType.ReflectionType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

            if (constructors.Length != 0)
            {
                failingTypes.Add(entityType);
            }
        }

        // Assert
        Assert.Empty(failingTypes);
    }

    [Fact]
    public void Domain_Entities_ShouldBeInCorrectNamespace()
    {
        // Arrange
        var conditionList = Types.InAssemblies(Assemblies.Domain)
            .That().Inherit<BaseEntity>()
            .Should().ResideInNamespaceContaining(Namespaces.PatternForEntities);

        // Act
        var result = conditionList.GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Domain_Entities_ShouldBeSealed()
    {
        // Arrange
        var conditionList = Types.InAssemblies(Assemblies.Domain)
            .That().Inherit<BaseEntity>()
            .And().AreNotAbstract()
            .Should().BeSealed();

        // Act
        var result = conditionList.GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }
}