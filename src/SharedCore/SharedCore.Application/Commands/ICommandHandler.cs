using SharedCore.Application.Commands.Models;

namespace SharedCore.Application.Commands;

/// <summary>
/// The command handler (without input).
/// </summary>
/// <typeparam name="TOutData">The type of the output data.</typeparam>
public interface ICommandHandler<TOutData>
{
    /// <summary>
    /// Handles the command asynchronous.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The command output.</returns>
    Task<CommandOut<TOutData>> HandleAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// The command handler.
/// </summary>
/// <typeparam name="TIn">The type of the input.</typeparam>
/// <typeparam name="TOutData">The type of the output data.</typeparam>
public interface ICommandHandler<in TIn, TOutData>
{
    /// <summary>
    /// Handles the input asynchronous.
    /// </summary>
    /// <param name="commandIn">The command input.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The command output.</returns>
    Task<CommandOut<TOutData>> HandleAsync(TIn commandIn, CancellationToken cancellationToken = default);
}