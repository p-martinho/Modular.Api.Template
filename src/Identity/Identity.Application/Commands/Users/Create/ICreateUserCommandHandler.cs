using Identity.Application.Dtos.Users;
using Identity.Application.Dtos.Users.Create;
using SharedCore.Application.Commands;

namespace Identity.Application.Commands.Users.Create;

/// <summary>
/// The create user command handler.
/// </summary>
/// <seealso cref="ICommandHandler{TIn,TOutData}"/>
public interface ICreateUserCommandHandler : ICommandHandler<CreateUserDto, UserInfoDto>;