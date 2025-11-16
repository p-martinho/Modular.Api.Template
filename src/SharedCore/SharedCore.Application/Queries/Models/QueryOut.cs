using System.Diagnostics.CodeAnalysis;
using SharedCore.Application.Common;
using SharedCore.Application.Common.Models;

namespace SharedCore.Application.Queries.Models;

/// <summary>
/// The query output.
/// </summary>
/// <typeparam name="TOutData">The type of the output data.</typeparam>
/// <param name="Result">The output result.</param>
/// <param name="Data">The output data.</param>
/// <remarks>The output data can be <c>default</c>, for instance, in case of an error.</remarks>
[ExcludeFromCodeCoverage]
public record QueryOut<TOutData>(OutputResult Result, TOutData? Data = default)
{
    /// <summary>
    /// Builds a query output with the result type <see cref="ResultType.Success"/>.
    /// </summary>
    /// <param name="data">The output data.</param>
    /// <param name="message">The message (optional).</param>
    /// <returns>The query output.</returns>
    public static QueryOut<TOutData> Success(TOutData data, string? message = null) =>
        new(OutputResultBuilder.BuildSuccess(message), data);

    /// <summary>
    /// Builds a query output with the result type <see cref="ResultType.NotFoundError"/>.
    /// </summary>
    /// <param name="message">The message (optional).</param>
    /// <returns>The query output.</returns>
    public static QueryOut<TOutData> NotFoundError(string? message = null) =>
        new(OutputResultBuilder.BuildNotFoundError(message));

    /// <summary>
    /// Builds a query output with the result type <see cref="ResultType.ValidationError"/>.
    /// </summary>
    /// <param name="message">The message (optional).</param>
    /// <param name="resultDetails">The output result details.</param>
    /// <returns>The query output.</returns>
    public static QueryOut<TOutData> ValidationError(string? message = null,
        IEnumerable<ResultDetail>? resultDetails = null) =>
        new(OutputResultBuilder.BuildValidationError(message, resultDetails));

    /// <summary>
    /// Builds a query output with the result type <see cref="ResultType.AuthorizationError"/>.
    /// </summary>
    /// <param name="message">The message (optional).</param>
    /// <returns>The query output.</returns>
    public static QueryOut<TOutData> AuthorizationError(string? message = null) =>
        new(OutputResultBuilder.BuildAuthorizationError(message));

    /// <summary>
    /// Builds a query output with the result type <see cref="ResultType.InternalError"/>.
    /// </summary>
    /// <param name="message">The message (optional).</param>
    /// <returns>The query output.</returns>
    public static QueryOut<TOutData> InternalError(string? message = null) =>
        new(OutputResultBuilder.BuildInternalError(message));

    /// <summary>
    /// Builds a query output with the result type <see cref="ResultType.OperationCanceled"/>.
    /// </summary>
    /// <param name="message">The message (optional).</param>
    /// <returns>The query output.</returns>
    public static QueryOut<TOutData> OperationCanceled(string? message = null) =>
        new(OutputResultBuilder.BuildOperationCanceled(message));
}