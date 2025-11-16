using System.Reflection;
using Identity.Application.Commands.Users.Create;
using Identity.Domain.Entities.Users;
using Identity.Persistence;
using SharedCore.Application.Commands;
using SharedCore.Common.ApplicationContext;
using SharedCore.Domain.Entities;
using SharedCore.Persistence;
using SharedCore.Presentation.Endpoints;
using Todo.Application.Commands.TodoLists.Create;
using Todo.Domain.Entities.TodoLists;
using Todo.Persistence;

namespace Architecture.Tests.Helpers;

/// <summary>
/// Assemblies references for every module (including SharedCore assemblies).
/// </summary>
/// <remarks>Add here the assemblies for each module.</remarks>
internal static class Assemblies
{
    public static readonly Assembly[] Presentation =
    [
        typeof(IEndpointGroup).Assembly,
        typeof(Todo.Presentation.Api.DependencyInjection.DependencyInjectionExtensions).Assembly,
        typeof(Identity.Presentation.Api.DependencyInjection.DependencyInjectionExtensions).Assembly
    ];

    public static readonly Assembly[] Application =
    [
        typeof(ICommandHandler<,>).Assembly,
        typeof(ICreateTodoListCommandHandler).Assembly,
        typeof(ICreateUserCommandHandler).Assembly
    ];

    public static readonly Assembly[] Persistence =
    [
        typeof(BaseDbContext<>).Assembly,
        typeof(TodoDbContext).Assembly,
        typeof(IdentityDbContext).Assembly
    ];

    public static readonly Assembly[] Domain =
    [
        typeof(BaseEntity).Assembly,
        typeof(TodoList).Assembly,
        typeof(AppIdentityUser).Assembly
    ];

    public static readonly Assembly[] Common =
    [
        typeof(ICurrentUser).Assembly,
        typeof(Todo.Common.DependencyInjection.DependencyInjectionExtensions).Assembly,
        typeof(Identity.Common.DependencyInjection.DependencyInjectionExtensions).Assembly
    ];

    public static readonly Assembly[] All =
        [.. Presentation, .. Application, .. Persistence, .. Domain, .. Common];

    public static Assembly[] GetAllExcept(params Assembly[] assemblies)
    {
        return All.Where(a => !assemblies.Contains(a)).ToArray();
    }
}