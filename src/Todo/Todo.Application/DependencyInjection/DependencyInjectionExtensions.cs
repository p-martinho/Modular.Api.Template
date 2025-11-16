using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedCore.Application.DependencyInjection;
using Todo.Application.Commands.TodoLists.Create;
using Todo.Application.Commands.TodoLists.Delete;
using Todo.Application.Commands.TodoLists.TodoItems.Create;
using Todo.Application.Commands.TodoLists.TodoItems.Delete;
using Todo.Application.Commands.TodoLists.TodoItems.Update;
using Todo.Application.Commands.TodoLists.Update;
using Todo.Application.Dtos.TodoLists.Create;
using Todo.Application.Dtos.TodoLists.TodoItems.Create;
using Todo.Application.Dtos.TodoLists.TodoItems.Update;
using Todo.Application.Dtos.TodoLists.Update;
using Todo.Application.Queries.TodoLists.Get;
using Todo.Application.Queries.TodoLists.GetById;
using Todo.Persistence.DependencyInjection;

namespace Todo.Application.DependencyInjection;

/// <summary>
/// The dependency injection extensions.
/// </summary>
[ExcludeFromCodeCoverage]
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Adds the application dependencies.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSharedApplication(configuration);

        services.AddPersistence(configuration);

        services.AddValidators();

        services.AddCommandHandlers();

        services.AddQueryHandlers();

        return services;
    }

    /// <summary>
    /// Adds the application health checks.
    /// </summary>
    /// <param name="healthChecksBuilder">The health checks builder.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The health checks builder.</returns>
    public static IHealthChecksBuilder AddApplicationHealthChecks(this IHealthChecksBuilder healthChecksBuilder,
        IConfiguration configuration)
    {
        healthChecksBuilder.AddPersistenceHealthChecks(configuration);

        return healthChecksBuilder;
    }

    private static void AddValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CreateTodoListDto>, CreateTodoListValidator>();
        services.AddScoped<IValidator<UpdateTodoListDto>, UpdateTodoListValidator>();

        services.AddScoped<IValidator<CreateTodoItemDto>, CreateTodoItemValidator>();
        services.AddScoped<IValidator<UpdateTodoItemDto>, UpdateTodoItemValidator>();
    }

    private static void AddCommandHandlers(this IServiceCollection services)
    {
        services.AddScoped<ICreateTodoListCommandHandler, CreateTodoListCommandHandler>();
        services.AddScoped<IUpdateTodoListCommandHandler, UpdateTodoListCommandHandler>();
        services.AddScoped<IDeleteTodoListCommandHandler, DeleteTodoListCommandHandler>();

        services.AddScoped<ICreateTodoItemCommandHandler, CreateTodoItemCommandHandler>();
        services.AddScoped<IUpdateTodoItemCommandHandler, UpdateTodoItemCommandHandler>();
        services.AddScoped<IDeleteTodoItemCommandHandler, DeleteTodoItemCommandHandler>();
    }

    private static void AddQueryHandlers(this IServiceCollection services)
    {
        services.AddScoped<IGetTodoListByIdQueryHandler, GetTodoListByIdQueryHandler>();
        services.AddScoped<IGetTodoListsQueryHandler, GetTodoListsQueryHandler>();
    }
}