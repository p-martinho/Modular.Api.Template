using FluentValidation;
using Identity.Application.Dtos.Users;
using Identity.Application.Dtos.Users.Update;
using Identity.Application.MappingExtensions.Users;
using Identity.Domain.Entities.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SharedCore.Application.Commands;
using SharedCore.Application.Commands.Models;
using SharedCore.Common.ApplicationContext;
using SharedCore.Common.Authorization;

namespace Identity.Application.Commands.Users.Update;

/// <summary>
/// The update user info command handler.
/// </summary>
/// <seealso cref="CommandHandler{TIn,TOutData}"/>
/// <seealso cref="IUpdateUserInfoCommandHandler"/>
internal class UpdateUserInfoCommandHandler : CommandHandler<UpdateUserInfoDto, UserInfoDto>,
    IUpdateUserInfoCommandHandler
{
    private readonly ICurrentUser _currentUser;
    private readonly UserManager<AppIdentityUser> _userManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateUserInfoCommandHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="validator">The validator.</param>
    /// <param name="currentUser">The current user.</param>
    /// <param name="userManager">The user manager.</param>
    public UpdateUserInfoCommandHandler(ILogger<UpdateUserInfoCommandHandler> logger,
        IValidator<UpdateUserInfoDto> validator,
        ICurrentUser currentUser,
        UserManager<AppIdentityUser> userManager)
        : base(logger, validator)
    {
        _currentUser = currentUser;
        _userManager = userManager;
    }

    /// <inheritdoc />
    protected override async Task<CommandOut<UserInfoDto>> HandleCommandInAsync(UpdateUserInfoDto commandIn,
        CancellationToken cancellationToken)
    {
        if (!HasPermissionsForOperation(commandIn.Id))
        {
            return CommandOut<UserInfoDto>.NotFoundError();
        }

        var user = await _userManager.FindByIdAsync(commandIn.Id);

        if (user is null)
        {
            return CommandOut<UserInfoDto>.NotFoundError();
        }

        var validationError = await UpdateEmailAsync(commandIn, user);

        if (validationError is not null)
        {
            return validationError;
        }

        validationError = await UpdateNameAsync(commandIn, user);

        return validationError ?? CommandOut<UserInfoDto>.Success(user.ToDto());
    }

    private bool HasPermissionsForOperation(string userId)
    {
        return _currentUser.UserId == userId || _currentUser.IsInRole(UserRoles.Admin);
    }

    private async Task<CommandOut<UserInfoDto>?> UpdateEmailAsync(UpdateUserInfoDto commandIn, AppIdentityUser user)
    {
        if (commandIn.Email is null || commandIn.Email == user.Email)
        {
            return null;
        }

        // Note: the email is being automatically confirmed here.
        // To confirm the email, would need to send the code to the email and create an endpoint to confirm the email.
        // Check how Identity does it here: https://github.com/dotnet/aspnetcore/blob/main/src/Identity/Core/src/IdentityApiEndpointRouteBuilderExtensions.cs.
        var code = await _userManager.GenerateChangeEmailTokenAsync(user, commandIn.Email);

        var result = await _userManager.ChangeEmailAsync(user, commandIn.Email, code);

        if (result.Succeeded)
        {
            result = await _userManager.SetUserNameAsync(user, commandIn.Email);
        }

        return GetValidationError(result);
    }

    private async Task<CommandOut<UserInfoDto>?> UpdateNameAsync(UpdateUserInfoDto commandIn, AppIdentityUser user)
    {
        if ((commandIn.FirstName is null || commandIn.FirstName == user.Name.FirstName) &&
            (commandIn.LastName is null || commandIn.LastName == user.Name.LastName))
        {
            return null;
        }

        user.UpdateName(commandIn.FirstName ?? user.Name.FirstName, commandIn.LastName ?? user.Name.LastName);

        var result = await _userManager.UpdateAsync(user);

        return GetValidationError(result);
    }

    private static CommandOut<UserInfoDto>? GetValidationError(IdentityResult result)
    {
        return result.Succeeded
            ? null
            : CommandOut<UserInfoDto>.ValidationError(resultDetails: result.Errors.Select(e => e.ToResultDetail()));
    }
}