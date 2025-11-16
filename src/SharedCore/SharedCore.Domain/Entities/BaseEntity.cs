using System.Diagnostics.CodeAnalysis;

namespace SharedCore.Domain.Entities;

/// <summary>
/// The base entity.
/// </summary>
[ExcludeFromCodeCoverage]
public abstract class BaseEntity
{
    /// <summary>
    /// The entity identifier.
    /// </summary>
    public Guid Id { get; private set; } = Guid.CreateVersion7();
}