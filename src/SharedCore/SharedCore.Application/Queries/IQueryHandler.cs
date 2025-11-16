using SharedCore.Application.Queries.Models;

namespace SharedCore.Application.Queries;

/// <summary>
/// The query handler (without input).
/// </summary>
/// <typeparam name="TOutData">The type of the output data.</typeparam>
public interface IQueryHandler<TOutData>
{
    /// <summary>
    /// Handles the query asynchronous.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The query output.</returns>
    Task<QueryOut<TOutData>> HandleAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// The query handler.
/// </summary>
/// <typeparam name="TIn">The type of the input.</typeparam>
/// <typeparam name="TOutData">The type of the output data.</typeparam>
public interface IQueryHandler<in TIn, TOutData>
{
    /// <summary>
    /// Handles the input asynchronous.
    /// </summary>
    /// <param name="queryIn">The query input.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The query output.</returns>
    Task<QueryOut<TOutData>> HandleAsync(TIn queryIn, CancellationToken cancellationToken = default);
}