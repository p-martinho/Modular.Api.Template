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
    /// The <see cref="IServiceCollection"/> extensions.
    /// </summary>
    /// <param name="services">The service collection.</param>
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Adds the application dependencies.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The service collection.</returns>
        public IServiceCollection AddApplication(IConfiguration configuration)
        {
            services.AddSharedApplication(configuration);

            services.AddPersistence(configuration);

            services.AddValidators();

            services.AddCommandHandlers();

            services.AddQueryHandlers();

            return services;
        }

        private void AddValidators()
        {
            services.AddScoped<IValidator<CreateTodoListDto>, CreateTodoListValidator>();
            services.AddScoped<IValidator<UpdateTodoListDto>, UpdateTodoListValidator>();

            services.AddScoped<IValidator<CreateTodoItemDto>, CreateTodoItemValidator>();
            services.AddScoped<IValidator<UpdateTodoItemDto>, UpdateTodoItemValidator>();
        }

        private void AddCommandHandlers()
        {
            services.AddScoped<ICreateTodoListCommandHandler, CreateTodoListCommandHandler>();
            services.AddScoped<IUpdateTodoListCommandHandler, UpdateTodoListCommandHandler>();
            services.AddScoped<IDeleteTodoListCommandHandler, DeleteTodoListCommandHandler>();

            services.AddScoped<ICreateTodoItemCommandHandler, CreateTodoItemCommandHandler>();
            services.AddScoped<IUpdateTodoItemCommandHandler, UpdateTodoItemCommandHandler>();
            services.AddScoped<IDeleteTodoItemCommandHandler, DeleteTodoItemCommandHandler>();
        }

        private void AddQueryHandlers()
        {
            services.AddScoped<IGetTodoListByIdQueryHandler, GetTodoListByIdQueryHandler>();
            services.AddScoped<IGetTodoListsQueryHandler, GetTodoListsQueryHandler>();
        }
    }

    /// <summary>
    /// The <see cref="IHealthChecksBuilder"/> extensions.
    /// </summary>
    /// <param name="healthChecksBuilder">The health checks builder.</param>
    extension(IHealthChecksBuilder healthChecksBuilder)
    {
        /// <summary>
        /// Adds the application health checks.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The health checks builder.</returns>
        public IHealthChecksBuilder AddApplicationHealthChecks(IConfiguration configuration)
        {
            healthChecksBuilder.AddPersistenceHealthChecks(configuration);

            return healthChecksBuilder;
        }
    }
}