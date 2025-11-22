using Identity.Application.Dtos.Users;
using SharedCore.Application.Queries;

namespace Identity.Application.Queries.Users.GetById;

/// <summary>
/// The get user by id query handler.
/// </summary>
/// <seealso cref="IQueryHandler{TIn,TOut}"/>
public interface IGetUserByIdQueryHandler : IQueryHandler<string, UserInfoDto>;