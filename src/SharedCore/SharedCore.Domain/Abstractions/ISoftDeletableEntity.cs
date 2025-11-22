namespace SharedCore.Domain.Abstractions;

/// <summary>
/// Entity that is soft deletable.
/// </summary>
/// <remarks>Entities that implement this interface should not be deleted from the storage, but marked as deleted, using a shadow property: </remarks>
public interface ISoftDeletableEntity;