using Architecture.Tests.Extensions;
using Architecture.Tests.Helpers;
using Identity.Persistence.Configurations.Users;
using Microsoft.EntityFrameworkCore;
using NetArchTest.Rules;
using SharedCore.Persistence;
using SharedCore.Persistence.Configurations;
using SharedCore.Persistence.Repositories;

namespace Architecture.Tests;

public class PersistenceTests
{
    [Fact]
    public void Persistence_ShouldOnlyHaveSpecificDependencies()
    {
        // Arrange
        var conditionList = Types.InAssemblies(Assemblies.Persistence)
            .That().DoNotHaveNameStartingWith("<>z__ReadOnlySingleElementList")
            .And().DoNotHaveName("Enumerator") // filter out 2 specific auto generated classes
            .Should().OnlyHaveDependencyOn([
                Namespaces.System,
                Namespaces.Microsoft,
                ..Namespaces.Persistence,
                ..Namespaces.Domain,
                ..Namespaces.Common
            ]);

        // Act
        var result = conditionList.GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Persistence_ShouldNotBeReferencedBySpecificAssemblies()
    {
        // Arrange
        var conditionList = Types.InAssemblies([
                ..Assemblies.Presentation, ..Assemblies.Domain, ..Assemblies.Common
            ])
            .ShouldNot().HaveDependencyOnAny(Namespaces.Persistence);

        // Act
        var result = conditionList.GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Persistence_EfCore_ShouldOnlyBeReferencedByPersistence()
    {
        // Arrange
        var conditionList = Types.InAssemblies(Assemblies.GetAllExcept([.. Assemblies.Persistence]))
            .ShouldNot().HaveDependencyOnAny(Namespaces.MicrosoftEfCore);

        // Act
        var result = conditionList.GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Persistence_DbContexts_ShouldNotBePublic()
    {
        // Arrange
        var conditionList = Types.InAssemblies(Assemblies.Persistence)
            .That().Inherit(typeof(DbContext))
            .And().DoNotHaveName(typeof(BaseDbContext<>).Name)
            .ShouldNot().BePublic();

        // Act
        var result = conditionList.GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Persistence_EntityTypeConfigurations_ShouldInheritSharedBaseConfiguration()
    {
        // Arrange
        var conditionList = Types.InAssemblies(Assemblies.Persistence)
            .That().ImplementInterface(typeof(IEntityTypeConfiguration<>))
            .And().DoNotHaveName(GetBaseEntityTypeConfigurationsNames())
            .And().DoNotHaveName(nameof(AppIdentityUserConfiguration)) // this is a special case
            .Should().Inherit(typeof(BaseEntityConfiguration<>));

        // Act
        var result = conditionList.GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Persistence_EntityTypeConfigurationImplemetations_ShouldNotBePublic()
    {
        // Arrange
        var conditionList = Types.InAssemblies(Assemblies.Persistence)
            .That().ImplementInterface(typeof(IEntityTypeConfiguration<>))
            .And().DoNotHaveName(GetBaseEntityTypeConfigurationsNames())
            .ShouldNot().BePublic();

        // Act
        var result = conditionList.GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Persistence_EntityTypeConfigurations_ShouldHaveCorrectName()
    {
        // Arrange
        var conditionList = Types.InAssemblies(Assemblies.Persistence)
            .That().ImplementInterface(typeof(IEntityTypeConfiguration<>))
            .And().AreNotGeneric()
            .Should().HaveNameEndingWith(ClassNames.EfCoreEntityTypeConfigurationNameEnding);

        // Act
        var result = conditionList.GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Persistence_EntityTypeConfigurations_ShouldHaveCorrectNamespace()
    {
        // Arrange
        var conditionList = Types.InAssemblies(Assemblies.Persistence)
            .That().ImplementInterface(typeof(IEntityTypeConfiguration<>))
            .Should().ResideInNamespaceContaining(Namespaces.PatternForEntityTypeConfiguration);

        // Act
        var result = conditionList.GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Persistence_Repositories_ShouldBeInPersistenceAssembly()
    {
        // Arrange
        var conditionList = Types.InAssemblies(Assemblies.All)
            .That().ImplementInterface(typeof(IRepository<>))
            .Or().ImplementInterface(typeof(IQueryRepository<>))
            .Should().ResideInAnyOfNamespaces(Namespaces.Persistence);

        // Act
        var result = conditionList.GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Persistence_RepositoryImplementations_ShouldNotBePublic()
    {
        // Arrange
        var conditionList = Types.InAssemblies(Assemblies.Persistence)
            .That().ImplementInterface(typeof(IRepository<>))
            .And().AreClasses()
            .And().DoNotHaveName(typeof(Repository<>).Name)
            .ShouldNot().BePublic();

        // Act
        var result = conditionList.GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Persistence_QueryRepositoryImplementations_ShouldNotBePublic()
    {
        // Arrange
        var conditionList = Types.InAssemblies(Assemblies.Persistence)
            .That().ImplementInterface(typeof(IQueryRepository<>))
            .And().AreClasses()
            .And().DoNotHaveName(typeof(Repository<>).Name)
            .And().DoNotHaveName(typeof(QueryRepository<>).Name)
            .ShouldNot().BePublic();

        // Act
        var result = conditionList.GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Persistence_Repositories_ShouldHaveCorrectName()
    {
        // Arrange
        var conditionList = Types.InAssemblies(Assemblies.Persistence)
            .That().ImplementInterface(typeof(IRepository<>))
            .And().AreNotGeneric()
            .Should().HaveNameEndingWith(ClassNames.RepositoryNameEnding);

        // Act
        var result = conditionList.GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Persistence_Repositories_ShouldHaveCorrectNamespace()
    {
        // Arrange
        var conditionList = Types.InAssemblies(Assemblies.Persistence)
            .That().ImplementInterface(typeof(IRepository<>))
            .Should().ResideInNamespaceContaining(Namespaces.PatternForRepository);

        // Act
        var result = conditionList.GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    private static string[] GetBaseEntityTypeConfigurationsNames()
    {
        return
        [
            typeof(BaseEntityConfiguration<>).Name,
            typeof(BaseAuditableEntityConfiguration<>).Name,
            typeof(BaseOwnedEntityConfiguration<>).Name
        ];
    }
}