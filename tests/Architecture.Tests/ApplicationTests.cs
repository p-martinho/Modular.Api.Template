using Architecture.Tests.Extensions;
using Architecture.Tests.Helpers;
using NetArchTest.Rules;
using SharedCore.Application.Commands;
using SharedCore.Application.Queries;

namespace Architecture.Tests;

public class ApplicationTests
{
    [Fact]
    public void Application_ShouldNotBeReferencedBySpecificAssemblies()
    {
        // Arrange
        var conditionList = Types
            .InAssemblies([.. Assemblies.Persistence, .. Assemblies.Common, .. Assemblies.Domain])
            .ShouldNot().HaveDependencyOnAny(Namespaces.Application);

        // Act
        var result = conditionList.GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Application_CommandHandlers_ShouldBeInApplicationAssembly()
    {
        // Arrange
        var conditionList = Types.InAssemblies(Assemblies.All)
            .That().ImplementInterface(typeof(ICommandHandler<,>))
            .Or().ImplementInterface(typeof(ICommandHandler<>))
            .Should().ResideInAnyOfNamespaces(Namespaces.Application);

        // Act
        var result = conditionList.GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Application_QueryHandlers_ShouldBeInApplicationAssembly()
    {
        // Arrange
        var conditionList = Types.InAssemblies(Assemblies.All)
            .That().ImplementInterface(typeof(IQueryHandler<,>))
            .Or().ImplementInterface(typeof(IQueryHandler<>))
            .Should().ResideInAnyOfNamespaces(Namespaces.Application);

        // Act
        var result = conditionList.GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Application_CommandHandlerImplementations_ShouldNotBePublic()
    {
        // Arrange
        var conditionList = Types.InAssemblies(Assemblies.Application)
            .That().ImplementInterface(typeof(ICommandHandler<,>))
            .And().AreClasses()
            .And().DoNotHaveName(typeof(CommandHandler<,>).Name)
            .ShouldNot().BePublic();

        // Act
        var result = conditionList.GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Application_CommandHandlerWithoutInputImplementations_ShouldNotBePublic()
    {
        // Arrange
        var conditionList = Types.InAssemblies(Assemblies.Application)
            .That().ImplementInterface(typeof(ICommandHandler<>))
            .And().AreClasses()
            .And().DoNotHaveName(typeof(CommandHandler<>).Name)
            .ShouldNot().BePublic();

        // Act
        var result = conditionList.GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Application_QueryHandlerImplementations_ShouldNotBePublic()
    {
        // Arrange
        var conditionList = Types.InAssemblies(Assemblies.Application)
            .That().ImplementInterface(typeof(IQueryHandler<,>))
            .And().AreClasses()
            .And().DoNotHaveName(typeof(QueryHandler<,>).Name)
            .ShouldNot().BePublic();

        // Act
        var result = conditionList.GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Application_QueryHandlerWithoutInputImplementations_ShouldNotBePublic()
    {
        // Arrange
        var conditionList = Types.InAssemblies(Assemblies.Application)
            .That().ImplementInterface(typeof(IQueryHandler<>))
            .And().AreClasses()
            .And().DoNotHaveName(typeof(QueryHandler<>).Name)
            .ShouldNot().BePublic();

        // Act
        var result = conditionList.GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Application_CommandHandlers_ShouldHaveCorrectName()
    {
        // Arrange
        var conditionList = Types.InAssemblies(Assemblies.Application)
            .That().ImplementInterface(typeof(ICommandHandler<,>))
            .And().AreNotGeneric()
            .Should().HaveNameEndingWith(ClassNames.CommandHandlerNameEnding);

        // Act
        var result = conditionList.GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Application_CommandHandlersWithoutInput_ShouldHaveCorrectName()
    {
        // Arrange
        var conditionList = Types.InAssemblies(Assemblies.Application)
            .That().ImplementInterface(typeof(ICommandHandler<>))
            .And().AreNotGeneric()
            .Should().HaveNameEndingWith(ClassNames.CommandHandlerNameEnding);

        // Act
        var result = conditionList.GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Application_QueryHandlers_ShouldHaveCorrectName()
    {
        // Arrange
        var conditionList = Types.InAssemblies(Assemblies.Application)
            .That().ImplementInterface(typeof(IQueryHandler<,>))
            .And().AreNotGeneric()
            .Should().HaveNameEndingWith(ClassNames.QueryHandlerNameEnding);

        // Act
        var result = conditionList.GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Application_QueryHandlersWithoutInput_ShouldHaveCorrectName()
    {
        // Arrange
        var conditionList = Types.InAssemblies(Assemblies.Application)
            .That().ImplementInterface(typeof(IQueryHandler<>))
            .And().AreNotGeneric()
            .Should().HaveNameEndingWith(ClassNames.QueryHandlerNameEnding);

        // Act
        var result = conditionList.GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Application_CommandHandlers_ShouldHaveCorrectNamespace()
    {
        // Arrange
        var conditionList = Types.InAssemblies(Assemblies.Application)
            .That().ImplementInterface(typeof(ICommandHandler<,>))
            .Or().ImplementInterface(typeof(ICommandHandler<>))
            .Should().ResideInNamespaceContaining(Namespaces.PatternForCommandHandler);

        // Act
        var result = conditionList.GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Application_QueryHandlers_ShouldHaveCorrectNamespace()
    {
        // Arrange
        var conditionList = Types.InAssemblies(Assemblies.Application)
            .That().ImplementInterface(typeof(IQueryHandler<,>))
            .Or().ImplementInterface(typeof(IQueryHandler<>))
            .Should().ResideInNamespaceContaining(Namespaces.PatternForQueryHandler);

        // Act
        var result = conditionList.GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Application_ShouldOnlyHaveHandlerInterfacesAndDtosAsPublic()
    {
        // Arrange
        var dtos = Types.InAssemblies(Assemblies.Application)
            .That().HaveNameEndingWith(ClassNames.ApplicationDtoNameEnding)
            .GetTypes()
            .Select(t => t.ReflectionType);

        var commandHandlersWithoutInputInterfaces = Types.InAssemblies(Assemblies.Application)
            .That().ImplementInterface(typeof(ICommandHandler<>))
            .And().AreInterfaces()
            .GetTypes()
            .Select(t => t.ReflectionType);

        var commandHandlersInterfaces = Types.InAssemblies(Assemblies.Application)
            .That().ImplementInterface(typeof(ICommandHandler<,>))
            .And().AreInterfaces()
            .GetTypes()
            .Select(t => t.ReflectionType);

        var queryHandlersInterfaces = Types.InAssemblies(Assemblies.Application)
            .That().ImplementInterface(typeof(IQueryHandler<,>))
            .And().AreInterfaces()
            .GetTypes()
            .Select(t => t.ReflectionType);

        var queryHandlersWithoutInputInterfaces = Types.InAssemblies(Assemblies.Application)
            .That().ImplementInterface(typeof(IQueryHandler<>))
            .And().AreInterfaces()
            .GetTypes()
            .Select(t => t.ReflectionType);

        var allPublicApplicationTypes = Types.InAssemblies(Assemblies.Application)
            .That().DoNotResideInNamespaceStartingWith(Namespaces.SharedCoreStarting)
            .And().DoNotHaveName(ClassNames.DependencyInjectionExtensions)
            .And().ArePublic()
            .GetTypes()
            .Select(t => t.ReflectionType);

        // Act
        var failingTypes = allPublicApplicationTypes.Where(t =>
            !dtos.Contains(t) &&
            !commandHandlersWithoutInputInterfaces.Contains(t) &&
            !commandHandlersInterfaces.Contains(t) &&
            !queryHandlersInterfaces.Contains(t) &&
            !queryHandlersWithoutInputInterfaces.Contains(t));

        // Assert
        Assert.Empty(failingTypes);
    }
}