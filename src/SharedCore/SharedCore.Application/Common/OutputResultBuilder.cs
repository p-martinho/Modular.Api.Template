using System.Diagnostics.CodeAnalysis;
using SharedCore.Application.Common.Models;

namespace SharedCore.Application.Common;

/// <summary>
/// The output result builder.
/// </summary>
[ExcludeFromCodeCoverage]
public static class OutputResultBuilder
{
    /// <summary>
    /// Builds an output result with type <see cref="ResultType.Success"/>.
    /// </summary>
    /// <param name="message">The message (optional).</param>
    /// <returns>The output result.</returns>
    public static OutputResult BuildSuccess(string? message = null) =>
        new(ResultType.Success, message ?? "Operation executed successfully.");

    /// <summary>
    /// Builds an output result with type <see cref="ResultType.NotFoundError"/>.
    /// </summary>
    /// <param name="message">The message (optional).</param>
    /// <returns>The output result.</returns>
    public static OutputResult BuildNotFoundError(string? message = null) =>
        new(ResultType.NotFoundError, message ?? "Resource not found.");

    /// <summary>
    /// Builds an output result with type <see cref="ResultType.ValidationError"/>.
    /// </summary>
    /// <param name="message">The message (optional).</param>
    /// <param name="resultDetails">The output result details (optional).</param>
    /// <returns>The output result.</returns>
    public static OutputResult BuildValidationError(string? message = null,
        IEnumerable<ResultDetail>? resultDetails = null) =>
        new(ResultType.ValidationError, message ?? "A validation error has occurred.", resultDetails);

    /// <summary>
    /// Builds an output result with type <see cref="ResultType.AuthorizationError"/>.
    /// </summary>
    /// <param name="message">The message (optional).</param>
    /// <returns>The output result.</returns>
    public static OutputResult BuildAuthorizationError(string? message = null) =>
        new(ResultType.AuthorizationError, message ?? "Insufficient permissions for the operation.");

    /// <summary>
    /// Builds an output result with type <see cref="ResultType.InternalError"/>.
    /// </summary>
    /// <param name="message">The message (optional).</param>
    /// <returns>The output result.</returns>
    public static OutputResult BuildInternalError(string? message = null) =>
        new(ResultType.InternalError, message ?? "An unexpected error has occurred.");

    /// <summary>
    /// Builds an output result with type <see cref="ResultType.OperationCanceled"/>.
    /// </summary>
    /// <param name="message">The message (optional).</param>
    /// <returns>The output result.</returns>
    public static OutputResult BuildOperationCanceled(string? message = null) =>
        new(ResultType.OperationCanceled, message ?? "The operation has been canceled.");
}