namespace SharedCore.Application.Common.Models;

/// <summary>
/// The result type.
/// </summary>
public enum ResultType
{
    /// <summary>
    /// The successful result.
    /// </summary>
    Success,

    /// <summary>
    /// The resource was not found.
    /// </summary>
    NotFoundError,

    /// <summary>
    /// A validation error has occured.
    /// </summary>
    ValidationError,

    /// <summary>
    /// The user does not have the permissions for the operation.
    /// </summary>
    AuthorizationError,

    /// <summary>
    /// An internal error occured. 
    /// </summary>
    InternalError,

    /// <summary>
    /// The operation was cancelled.
    /// </summary>
    OperationCanceled
}