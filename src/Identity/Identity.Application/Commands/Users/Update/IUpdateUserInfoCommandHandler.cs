using Identity.Application.Dtos.Users;
using Identity.Application.Dtos.Users.Update;
using SharedCore.Application.Commands;

namespace Identity.Application.Commands.Users.Update;

/// <summary>
/// The update user info command handler.
/// </summary>
/// <seealso cref="ICommandHandler{TIn,TOutData}"/>
public interface IUpdateUserInfoCommandHandler : ICommandHandler<UpdateUserInfoDto, UserInfoDto>;