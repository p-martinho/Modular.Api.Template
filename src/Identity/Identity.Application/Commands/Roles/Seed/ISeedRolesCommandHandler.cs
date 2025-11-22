using SharedCore.Application.Commands;

namespace Identity.Application.Commands.Roles.Seed;

/// <summary>
/// The seed roles command handler.
/// </summary>
/// <seealso cref="ICommandHandler{TOutData}"/>
public interface ISeedRolesCommandHandler : ICommandHandler<bool>;