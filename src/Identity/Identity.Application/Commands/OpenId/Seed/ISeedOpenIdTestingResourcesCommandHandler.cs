using SharedCore.Application.Commands;

namespace Identity.Application.Commands.OpenId.Seed;

/// <summary>
/// The seed OpenId testing resources command handler.
/// </summary>
/// <seealso cref="ICommandHandler{TOutData}"/>
public interface ISeedOpenIdTestingResourcesCommandHandler : ICommandHandler<bool>;