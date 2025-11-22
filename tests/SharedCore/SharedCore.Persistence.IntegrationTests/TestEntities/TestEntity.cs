using SharedCore.Domain.Abstractions;
using SharedCore.Domain.Entities;

namespace SharedCore.Persistence.IntegrationTests.TestEntities;

public class TestEntity : BaseOwnedEntity, IAggregateEntity, ISoftDeletableEntity
{
    public string Code { get; set; } = null!;

    public IList<TestChildEntity> Children { get; set; } = null!;

    public TestOwnedEntity OwnedEntity { get; set; } = new();
}

public class TestOwnedEntity
{
    public string Description { get; set; } = string.Empty;
}