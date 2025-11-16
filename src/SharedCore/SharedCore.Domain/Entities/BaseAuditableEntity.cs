using System.Diagnostics.CodeAnalysis;

namespace SharedCore.Domain.Entities;

/// <summary>
/// The base auditable entity.
/// </summary>
/// <seealso cref="BaseEntity"/>
[ExcludeFromCodeCoverage]
public abstract class BaseAuditableEntity : BaseEntity
{
    /// <summary>
    /// The date the entity was created at.
    /// </summary>
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// The date the entity was updated at.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; private set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// The identifier of the user that created the entity.
    /// </summary>
    public string? CreatedBy { get; private set; } = null;

    /// <summary>
    /// The identifier of the user that updated the entity.
    /// </summary>
    public string? UpdatedBy { get; private set; } = null;
}