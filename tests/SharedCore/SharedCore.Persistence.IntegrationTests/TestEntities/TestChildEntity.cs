using SharedCore.Domain.Abstractions;
using SharedCore.Domain.Entities;

namespace SharedCore.Persistence.IntegrationTests.TestEntities;

public class TestChildEntity : BaseAuditableEntity, IAggregateEntity, ISoftDeletableEntity
{
    public string Code { get; set; } = null!;

    public TestEntity Parent { get; set; } = null!;
}