using System.Diagnostics.CodeAnalysis;

namespace SharedCore.Domain.Entities;

/// <summary>
/// The base entity owned by a user.
/// </summary>
/// <seealso cref="BaseAuditableEntity"/>
[ExcludeFromCodeCoverage]
public abstract class BaseOwnedEntity : BaseAuditableEntity
{
    /// <summary>
    /// The owner identifier.
    /// </summary>
    public required string OwnerId { get; init; }
}