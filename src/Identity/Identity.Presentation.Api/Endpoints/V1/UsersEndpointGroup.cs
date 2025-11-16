using Asp.Versioning;
using Asp.Versioning.Builder;
using Identity.Application.Commands.Users.Create;
using Identity.Application.Commands.Users.Update;
using Identity.Application.Queries.Users.GetById;
using Identity.Presentation.Api.Dtos.V1.Users;
using Identity.Presentation.Api.Dtos.V1.Users.Create;
using Identity.Presentation.Api.Dtos.V1.Users.Update;
using Identity.Presentation.Api.MappingExtensions.V1.Users;
using Microsoft.AspNetCore.Http.HttpResults;
using SharedCore.Common.ApplicationContext;
using SharedCore.Presentation.Endpoints;
using SharedCore.Presentation.Extensions;
using SharedCore.Presentation.MappingExtensions;

namespace Identity.Presentation.Api.Endpoints.V1;

/// <summary>
/// The users endpoint group.
/// </summary>
/// <seealso cref="IEndpointGroup"/>
internal class UsersEndpointGroup : IEndpointGroup
{
    private const string EndpointGroupName = "Users";

    private static readonly ApiVersion ApiVersion = new(1, 0);

    /// <inheritdoc />
    public static void Map(WebApplication app, ApiVersionSet apiVersionSet)
    {
        var group = app.MapEndpointGroup(EndpointGroupName, apiVersionSet, ApiVersion, isAuthorizationRequired: true);

        group.MapPost("", CreateUserAsync)
            .AllowAnonymous()
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapGet("info", GetUserInfoAsync);

        group.MapPatch("info", UpdateUserInfoAsync)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapPut("password", UpdateUserPasswordAsync)
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    /// <summary>
    /// Creates the user.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="commandHandler">The command handler.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response result.</returns>
    public static async Task<Results<Created<UserInfoApiDto>, ProblemHttpResult>>
        CreateUserAsync(CreateUserApiDto request, ICreateUserCommandHandler commandHandler,
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
    /// Gets the logged user info.
    /// </summary>
    /// <param name="currentUser">The current user.</param>
    /// <param name="queryHandler">The query handler.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response result.</returns>
    public static async Task<Results<Ok<UserInfoApiDto>, UnauthorizedHttpResult, ProblemHttpResult>>
        GetUserInfoAsync(ICurrentUser currentUser, IGetUserByIdQueryHandler queryHandler,
            CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;

        if (userId is null)
        {
            return TypedResults.Unauthorized();
        }

        var queryOut = await queryHandler.HandleAsync(userId, cancellationToken);

        if (!queryOut.Result.IsSuccess)
        {
            return TypedResults.Problem(queryOut.Result.ToProblemDetails());
        }

        return TypedResults.Ok(queryOut.Data?.ToApiDto());
    }

    /// <summary>
    /// Updates the logged user info.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="currentUser">The current user.</param>
    /// <param name="commandHandler">The command handler.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response result.</returns>
    public static async Task<Results<Ok<UserInfoApiDto>, UnauthorizedHttpResult, ProblemHttpResult>>
        UpdateUserInfoAsync(UpdateUserInfoApiDto request, ICurrentUser currentUser,
            IUpdateUserInfoCommandHandler commandHandler, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;

        if (userId is null)
        {
            return TypedResults.Unauthorized();
        }

        var commandOut = await commandHandler.HandleAsync(request.ToDto(userId), cancellationToken);

        if (!commandOut.Result.IsSuccess)
        {
            return TypedResults.Problem(commandOut.Result.ToProblemDetails());
        }

        return TypedResults.Ok(commandOut.Data?.ToApiDto());
    }

    /// <summary>
    /// Updates the logged user password.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="currentUser">The current user.</param>
    /// <param name="commandHandler">The command handler.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response result.</returns>
    public static async Task<Results<Ok, UnauthorizedHttpResult, ProblemHttpResult>>
        UpdateUserPasswordAsync(UpdateUserPasswordApiDto request, ICurrentUser currentUser,
            IUpdateUserPasswordCommandHandler commandHandler, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;

        if (userId is null)
        {
            return TypedResults.Unauthorized();
        }

        var commandOut = await commandHandler.HandleAsync(request.ToDto(userId), cancellationToken);

        if (!commandOut.Result.IsSuccess)
        {
            return TypedResults.Problem(commandOut.Result.ToProblemDetails());
        }

        return TypedResults.Ok();
    }
}