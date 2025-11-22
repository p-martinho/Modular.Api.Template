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
/// The update user password command handler.
/// </summary>
/// <seealso cref="CommandHandler{TIn,TOutData}"/>
/// <seealso cref="IUpdateUserPasswordCommandHandler"/>
internal class UpdateUserPasswordCommandHandler : CommandHandler<UpdateUserPasswordDto, UserInfoDto>,
    IUpdateUserPasswordCommandHandler
{
    private readonly ICurrentUser _currentUser;
    private readonly UserManager<AppIdentityUser> _userManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateUserPasswordCommandHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="validator">The validator.</param>
    /// <param name="currentUser">The current user.</param>
    /// <param name="userManager">The user manager.</param>
    public UpdateUserPasswordCommandHandler(ILogger<UpdateUserPasswordCommandHandler> logger,
        IValidator<UpdateUserPasswordDto> validator,
        ICurrentUser currentUser,
        UserManager<AppIdentityUser> userManager)
        : base(logger, validator)
    {
        _currentUser = currentUser;
        _userManager = userManager;
    }

    /// <inheritdoc />
    protected override async Task<CommandOut<UserInfoDto>> HandleCommandInAsync(UpdateUserPasswordDto commandIn,
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

        var validationError = await UpdatePasswordAsync(commandIn, user);

        return validationError ?? CommandOut<UserInfoDto>.Success(user.ToDto());
    }

    private bool HasPermissionsForOperation(string userId)
    {
        return _currentUser.UserId == userId || _currentUser.IsInRole(UserRoles.Admin);
    }

    private async Task<CommandOut<UserInfoDto>?> UpdatePasswordAsync(UpdateUserPasswordDto commandIn, AppIdentityUser user)
    {
        var result = await _userManager.ChangePasswordAsync(user, commandIn.OldPassword, commandIn.NewPassword);

        return GetValidationError(result);
    }

    private static CommandOut<UserInfoDto>? GetValidationError(IdentityResult result)
    {
        return result.Succeeded
            ? null
            : CommandOut<UserInfoDto>.ValidationError(resultDetails: result.Errors.Select(e => e.ToResultDetail()));
    }
}