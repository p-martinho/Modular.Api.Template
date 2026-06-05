using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using SharedCore.Presentation.Dtos;
using SharedCore.Presentation.Endpoints;
using SharedCore.Presentation.Extensions;
using SharedCore.Presentation.MappingExtensions;
using Todo.Application.Commands.TodoLists.Create;
using Todo.Application.Commands.TodoLists.Delete;
using Todo.Application.Commands.TodoLists.TodoItems.Create;
using Todo.Application.Commands.TodoLists.TodoItems.Delete;
using Todo.Application.Commands.TodoLists.TodoItems.Update;
using Todo.Application.Commands.TodoLists.Update;
using Todo.Application.Dtos.TodoLists.TodoItems.Delete;
using Todo.Application.Queries.TodoLists.Get;
using Todo.Application.Queries.TodoLists.GetById;
using Todo.Presentation.Api.Dtos.V1.TodoLists;
using Todo.Presentation.Api.Dtos.V1.TodoLists.Create;
using Todo.Presentation.Api.Dtos.V1.TodoLists.TodoItems;
using Todo.Presentation.Api.Dtos.V1.TodoLists.TodoItems.Create;
using Todo.Presentation.Api.Dtos.V1.TodoLists.TodoItems.Update;
using Todo.Presentation.Api.Dtos.V1.TodoLists.Update;
using Todo.Presentation.Api.MappingExtensions.V1.TodoLists;

namespace Todo.Presentation.Api.Endpoints.V1;

/// <summary>
/// The To do lists endpoint group.
/// </summary>
/// <seealso cref="IEndpointGroup"/>
internal sealed class TodoListsEndpointGroup : IEndpointGroup
{
    private const string EndpointGroupName = "TodoLists";

    private static readonly ApiVersion ApiVersion = new(1, 0);

    /// <inheritdoc />
    public static void Map(IVersionedEndpointRouteBuilder apiBuilder)
    {
        var group = apiBuilder.MapEndpointGroup(EndpointGroupName, ApiVersion, isAuthorizationRequired: true);

        group.MapPost(string.Empty, CreateTodoListAsync)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapGet("{id:guid}", GetTodoListByIdAsync)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet(string.Empty, GetTodoListsAsync);

        group.MapPatch("{id:guid}", UpdateTodoListAsync)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapDelete("{id:guid}", DeleteTodoListAsync)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("{id:guid}/todoItems", CreateTodoItemAsync)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapPatch("{id:guid}/todoItems/{itemId:guid}", UpdateTodoItemAsync)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapDelete("{id:guid}/todoItems/{itemId:guid}", DeleteTodoItemAsync)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    /// <summary>
    /// Creates the to do list.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="commandHandler">The command handler.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response result.</returns>
    public static async Task<Results<Created<TodoListApiDto>, ProblemHttpResult>>
        CreateTodoListAsync(CreateTodoListApiDto request, ICreateTodoListCommandHandler commandHandler,
            CancellationToken cancellationToken)
    {
        var commandOut = await commandHandler.HandleAsync(request.ToDto(), cancellationToken);

        if (!commandOut.Result.IsSuccess)
        {
            return TypedResults.Problem(commandOut.Result.ToProblemDetails());
        }

        return TypedResults.Created($"api/{EndpointGroupName}/{commandOut.Data?.Id}", commandOut.Data?.ToApiDto());
    }

    /// <summary>
    /// Gets a to do list by its identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="queryHandler">The query handler.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response result.</returns>
    public static async Task<Results<Ok<TodoListApiDto>, ProblemHttpResult>>
        GetTodoListByIdAsync(Guid id, IGetTodoListByIdQueryHandler queryHandler, CancellationToken cancellationToken)
    {
        var queryOut = await queryHandler.HandleAsync(id, cancellationToken);

        if (!queryOut.Result.IsSuccess)
        {
            return TypedResults.Problem(queryOut.Result.ToProblemDetails());
        }

        return TypedResults.Ok(queryOut.Data?.ToApiDto());
    }

    /// <summary>
    /// Gets a collection of to do lists.
    /// </summary>
    /// <param name="query">The query parameters.</param>
    /// <param name="queryHandler">The query handler.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response result.</returns>
    public static async Task<Results<Ok<PaginatedQueryResultApiDto<TodoListApiDto>>, ProblemHttpResult>>
        GetTodoListsAsync([AsParameters] PaginatedQueryApiDto query, IGetTodoListsQueryHandler queryHandler,
            CancellationToken cancellationToken)
    {
        var queryOut = await queryHandler.HandleAsync(query.ToDto(), cancellationToken);

        if (!queryOut.Result.IsSuccess || queryOut.Data is null)
        {
            return TypedResults.Problem(queryOut.Result.ToProblemDetails());
        }

        return TypedResults.Ok(new PaginatedQueryResultApiDto<TodoListApiDto>
        {
            Pagination = queryOut.Data.Pagination.ToApiDto(),
            Records = queryOut.Data.Records.Select(d => d.ToApiDto())
        });
    }

    /// <summary>
    /// Updates a to do list.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="request">The request.</param>
    /// <param name="commandHandler">The command handler.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response result.</returns>
    public static async Task<Results<Ok<TodoListApiDto>, ProblemHttpResult>>
        UpdateTodoListAsync(Guid id, UpdateTodoListApiDto request, IUpdateTodoListCommandHandler commandHandler,
            CancellationToken cancellationToken)
    {
        var commandOut = await commandHandler.HandleAsync(request.ToDto(id), cancellationToken);

        if (!commandOut.Result.IsSuccess)
        {
            return TypedResults.Problem(commandOut.Result.ToProblemDetails());
        }

        return TypedResults.Ok(commandOut.Data?.ToApiDto());
    }

    /// <summary>
    /// Deletes a to do list.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="commandHandler">The command handler.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response result.</returns>
    public static async Task<Results<Ok<TodoListApiDto>, ProblemHttpResult>>
        DeleteTodoListAsync(Guid id, IDeleteTodoListCommandHandler commandHandler, CancellationToken cancellationToken)
    {
        var commandOut = await commandHandler.HandleAsync(id, cancellationToken);

        if (!commandOut.Result.IsSuccess)
        {
            return TypedResults.Problem(commandOut.Result.ToProblemDetails());
        }

        return TypedResults.Ok(commandOut.Data?.ToApiDto());
    }

    /// <summary>
    /// Creates a to do item in a list.
    /// </summary>
    /// <param name="id">The list identifier.</param>
    /// <param name="request">The request.</param>
    /// <param name="commandHandler">The command handler.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response result.</returns>
    public static async Task<Results<Created<TodoItemApiDto>, ProblemHttpResult>>
        CreateTodoItemAsync(Guid id, CreateTodoItemApiDto request, ICreateTodoItemCommandHandler commandHandler,
            CancellationToken cancellationToken)
    {
        var commandOut = await commandHandler.HandleAsync(request.ToDto(id), cancellationToken);

        if (!commandOut.Result.IsSuccess)
        {
            return TypedResults.Problem(commandOut.Result.ToProblemDetails());
        }

        return TypedResults.Created($"api/{EndpointGroupName}/{id}", commandOut.Data?.ToApiDto());
    }

    /// <summary>
    /// Updates a to do item in a list.
    /// </summary>
    /// <param name="id">The list identifier.</param>
    /// <param name="itemId">The to do item identifier.</param>
    /// <param name="request">The request.</param>
    /// <param name="commandHandler">The command handler.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response result.</returns>
    public static async Task<Results<Ok<TodoItemApiDto>, ProblemHttpResult>>
        UpdateTodoItemAsync(Guid id, Guid itemId, UpdateTodoItemApiDto request,
            IUpdateTodoItemCommandHandler commandHandler, CancellationToken cancellationToken)
    {
        var commandOut = await commandHandler.HandleAsync(request.ToDto(id, itemId), cancellationToken);

        if (!commandOut.Result.IsSuccess)
        {
            return TypedResults.Problem(commandOut.Result.ToProblemDetails());
        }

        return TypedResults.Ok(commandOut.Data?.ToApiDto());
    }

    /// <summary>
    /// Deletes a to do item in the list.
    /// </summary>
    /// <param name="id">The list identifier.</param>
    /// <param name="itemId">The to do item identifier.</param>
    /// <param name="commandHandler">The command handler.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response result.</returns>
    public static async Task<Results<Ok<TodoItemApiDto>, ProblemHttpResult>>
        DeleteTodoItemAsync(Guid id, Guid itemId, IDeleteTodoItemCommandHandler commandHandler,
            CancellationToken cancellationToken)
    {
        var commandIn = new DeleteTodoItemDto { ListId = id, Id = itemId };

        var commandOut = await commandHandler.HandleAsync(commandIn, cancellationToken);

        if (!commandOut.Result.IsSuccess)
        {
            return TypedResults.Problem(commandOut.Result.ToProblemDetails());
        }

        return TypedResults.Ok(commandOut.Data?.ToApiDto());
    }
}