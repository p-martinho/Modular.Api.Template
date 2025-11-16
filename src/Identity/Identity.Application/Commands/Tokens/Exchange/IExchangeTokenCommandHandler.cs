using System.Security.Claims;
using OpenIddict.Abstractions;
using SharedCore.Application.Commands;

namespace Identity.Application.Commands.Tokens.Exchange;

/// <summary>
/// The exchange token command handler.
/// </summary>
/// <seealso cref="ICommandHandler{TIn,TOutData}"/>
public interface IExchangeTokenCommandHandler : ICommandHandler<OpenIddictRequest, ClaimsPrincipal>;