using FluentValidation;
using Identity.Application.Dtos.Users;
using Identity.Application.Dtos.Users.Create;
using Identity.Application.MappingExtensions.Users;
using Identity.Domain.Entities.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SharedCore.Application.Commands;
using SharedCore.Application.Commands.Models;

namespace Identity.Application.Commands.Users.Create;

/// <summary>
/// The create user command handler.
/// </summary>
/// <seealso cref="CommandHandler{TIn,TOutData}"/>
/// <seealso cref="ICreateUserCommandHandler"/>
internal class CreateUserCommandHandler : CommandHandler<CreateUserDto, UserInfoDto>,
    ICreateUserCommandHandler
{
    private readonly UserManager<AppIdentityUser> _userManager;
    private readonly IUserStore<AppIdentityUser> _userStore;
    private readonly IUserEmailStore<AppIdentityUser> _userEmailStore;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateUserCommandHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="validator">The validator.</param>
    /// <param name="userManager">The user manager.</param>
    /// <param name="userStore">The user store.</param>
    public CreateUserCommandHandler(ILogger<CreateUserCommandHandler> logger,
        IValidator<CreateUserDto> validator,
        UserManager<AppIdentityUser> userManager,
        IUserStore<AppIdentityUser> userStore)
        : base(logger, validator)
    {
        _userManager = userManager;
        _userStore = userStore;
        _userEmailStore = (IUserEmailStore<AppIdentityUser>)userStore;
    }

    /// <inheritdoc />
    protected override async Task<CommandOut<UserInfoDto>> HandleCommandInAsync(CreateUserDto commandIn,
        CancellationToken cancellationToken)
    {
        var user = AppIdentityUser.Create(commandIn.Email, commandIn.FirstName, commandIn.LastName);

        if (user is null)
        {
            return CommandOut<UserInfoDto>.ValidationError();
        }

        await _userStore.SetUserNameAsync(user, user.Email, cancellationToken);
        await _userEmailStore.SetEmailAsync(user, user.Email, CancellationToken.None);

        var result = await _userManager.CreateAsync(user, commandIn.Password);

        if (!result.Succeeded)
        {
            return CommandOut<UserInfoDto>.ValidationError(resultDetails: result.Errors.Select(e => e.ToResultDetail()));
        }

        await ConfirmEmailAsync(user);

        return CommandOut<UserInfoDto>.Success(user.ToDto());
    }

    private async Task ConfirmEmailAsync(AppIdentityUser user)
    {
        // Note: the email is being automatically confirmed here.
        // To confirm the email, would need to send the code to the email and create an endpoint to confirm the email.
        // Check how Identity does it here: https://github.com/dotnet/aspnetcore/blob/main/src/Identity/Core/src/IdentityApiEndpointRouteBuilderExtensions.cs.

        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        await _userManager.ConfirmEmailAsync(user, code);
    }
}