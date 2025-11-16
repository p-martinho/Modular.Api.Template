using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SharedCore.Common.ApplicationContext;
using SharedCore.Persistence.Constants;
using SharedCore.Persistence.IntegrationTests.Fixtures;
using SharedCore.Persistence.IntegrationTests.TestConfigurations;
using SharedCore.Persistence.IntegrationTests.TestEntities;
using SharedCore.Persistence.IntegrationTests.TestServices;

namespace SharedCore.Persistence.IntegrationTests;

public class BaseDbContextTests : IClassFixture<EfCoreFixture>, IDisposable
{
    private readonly IServiceScope _testScope;
    private readonly TestDbContext _context;
    private readonly TestCurrentUser _currentUser;

    public BaseDbContextTests(EfCoreFixture fixture)
    {
        _testScope = fixture.ServiceProvider.CreateScope();
        _context = _testScope.ServiceProvider.GetRequiredService<TestDbContext>();
        _currentUser = (_testScope.ServiceProvider.GetRequiredService<ICurrentUser>() as TestCurrentUser)!;
    }

    [Fact]
    public void Initialization_ShouldApplyDefaultSchema()
    {
        // Arrange
        var defaultSchema = TestDbContext.GetDefaultSchema();

        // Act

        // Assert
        Assert.Equal(defaultSchema, _context.TestEntities.EntityType.GetSchema());
        Assert.Equal(defaultSchema, _context.TestChildEntities.EntityType.GetSchema());
    }

    [Fact]
    public void Initialization_ShouldApplyConfigurations()
    {
        // Arrange
        var testEntityTable = TestEntityConfiguration.GetTableName();
        var testChildEntityTable = TestChildEntityConfiguration.GetTableName();

        // Act

        // Assert
        Assert.Equal(testEntityTable, _context.TestEntities.EntityType.GetTableName());
        Assert.Equal(testChildEntityTable, _context.TestChildEntities.EntityType.GetTableName());
    }

    [Fact]
    public async Task AddAsync_ShouldAddToTheDatabase()
    {
        // Arrange
        var originalEntity = new TestEntity
        {
            OwnerId = "OwnerId",
            Code = Guid.NewGuid().ToString(),
            Children = new List<TestChildEntity>
            {
                new() { Code = Guid.NewGuid().ToString() }, new() { Code = Guid.NewGuid().ToString() }
            }
        };

        // Act
        _context.TestEntities.Add(originalEntity);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);
        _context.ChangeTracker.Clear();

        // Assert
        var fetchedEntity = await _context.TestEntities
            .Include(e => e.Children)
            .SingleOrDefaultAsync(e => e.Id == originalEntity.Id, TestContext.Current.CancellationToken);
        Assert.NotNull(fetchedEntity);
        Assert.NotSame(originalEntity, fetchedEntity);
        Assert.Equal(originalEntity.Code, fetchedEntity.Code);
        Assert.Equal(originalEntity.Children.Count, fetchedEntity.Children.Count);
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldUpdateEntityInTheDatabase()
    {
        // Arrange
        var originalEntity = new TestEntity { OwnerId = "OwnerId", Code = Guid.NewGuid().ToString() };
        _context.TestEntities.Add(originalEntity);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);
        _context.ChangeTracker.Clear();

        // Act
        var fetchedEntity = await _context.TestEntities
            .SingleAsync(e => e.Id == originalEntity.Id, TestContext.Current.CancellationToken);
        fetchedEntity.Code = Guid.NewGuid().ToString();
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);
        _context.ChangeTracker.Clear();

        // Assert
        var updatedEntity = await _context.TestEntities
            .SingleOrDefaultAsync(e => e.Id == originalEntity.Id, TestContext.Current.CancellationToken);
        Assert.NotNull(updatedEntity);
        Assert.NotSame(originalEntity, updatedEntity);
        Assert.NotEqual(originalEntity.Code, updatedEntity.Code);
    }

    [Fact]
    public async Task SaveChangesAsync_WhenInsertingAnEntity_ShouldSetAuditProperties()
    {
        // Arrange
        var userIdOnCreate = _currentUser.UserId;
        var entity = new TestEntity
        {
            OwnerId = "OwnerId",
            Code = Guid.NewGuid().ToString(),
            Children = new List<TestChildEntity> { new() { Code = Guid.NewGuid().ToString() } }
        };
        var originalCreatedDate = entity.CreatedAt;
        var originalUpdatedDate = entity.UpdatedAt;

        // Act
        _context.TestEntities.Add(entity);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Assert
        var currentCreatedDate = entity.CreatedAt;
        var currentUpdatedDate = entity.UpdatedAt;
        Assert.False(originalCreatedDate == default);
        Assert.False(originalUpdatedDate == default);
        Assert.Equal(currentCreatedDate, currentUpdatedDate);
        Assert.True(currentCreatedDate > originalCreatedDate); // The date was set again on the save changes
        Assert.True(currentCreatedDate > originalUpdatedDate); // The date was set again on the save changes
        Assert.Equal(userIdOnCreate, entity.CreatedBy);
        Assert.Equal(userIdOnCreate, entity.UpdatedBy);
        var childEntity = entity.Children.First();
        Assert.Equal(currentCreatedDate, childEntity.CreatedAt);
        Assert.Equal(currentCreatedDate, childEntity.UpdatedAt);
        Assert.Equal(userIdOnCreate, childEntity.CreatedBy);
        Assert.Equal(userIdOnCreate, childEntity.UpdatedBy);
    }

    [Fact]
    public async Task SaveChangesAsync_WhenUpdatingAnEntity_ShouldUpdateAuditProperties()
    {
        // Arrange
        var userIdOnCreate = _currentUser.UserId;
        const string userIdOnUpdate = "otherUserId";
        var entity = new TestEntity
        {
            OwnerId = "OwnerId",
            Code = Guid.NewGuid().ToString(),
            Children = new List<TestChildEntity> { new() { Code = Guid.NewGuid().ToString() } }
        };
        _context.TestEntities.Add(entity);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);
        var createdDateBeforeUpdate = entity.CreatedAt;
        var updatedDateBeforeUpdate = entity.UpdatedAt;

        // Act
        _currentUser.UserId = userIdOnUpdate;
        entity.Code = Guid.NewGuid().ToString();
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(createdDateBeforeUpdate, entity.CreatedAt); // The created at does not change with the update
        Assert.True(updatedDateBeforeUpdate < entity.UpdatedAt);
        Assert.Equal(userIdOnCreate, entity.CreatedBy); // The created by does not change with the update
        Assert.Equal(userIdOnUpdate, entity.UpdatedBy);
        var childEntity = entity.Children.First();
        Assert.Equal(updatedDateBeforeUpdate, childEntity.UpdatedAt); // The child entity was not updated, so the date does not change
        Assert.Equal(userIdOnCreate, childEntity.CreatedBy);
        Assert.Equal(userIdOnCreate, childEntity.UpdatedBy); // The child entity was not updated, so the updated by does not change
    }

    [Fact]
    public async Task Remove_ShouldApplySoftDelete()
    {
        // Arrange
        var userIdOnCreate = _currentUser.UserId;
        const string userIdOnRemove = "otherUserId";
        var entity = new TestEntity
        {
            OwnerId = "OwnerId",
            Code = Guid.NewGuid().ToString(),
            Children = new List<TestChildEntity> { new() { Code = Guid.NewGuid().ToString() } }
        };
        _context.TestEntities.Add(entity);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);
        var createdDateBeforeRemove = entity.CreatedAt;
        var updatedDateBeforeRemove = entity.UpdatedAt;

        // Act
        _currentUser.UserId = userIdOnRemove;
        _context.TestEntities.Remove(entity);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);
        _context.ChangeTracker.Clear();

        // Assert
        var removedEntity = await _context.TestEntities.FirstOrDefaultAsync(e => e.Id == entity.Id,
                TestContext.Current.CancellationToken);
        var removedChildEntity = await _context.TestChildEntities.FirstOrDefaultAsync(e => e.Parent.Id == entity.Id,
            TestContext.Current.CancellationToken);
        Assert.Null(removedEntity);
        Assert.Null(removedChildEntity);
        var fetchedRemovedEntity = _context.TestEntities
            .IgnoreQueryFilters()
            .Include(e => e.Children)
            .FirstOrDefault(e => e.Id == entity.Id);
        Assert.NotNull(fetchedRemovedEntity);
        var isEntityDeletedProperty = _context.Entry(fetchedRemovedEntity)
            .Property<bool>(EntityProperties.IsDeleted)
            .CurrentValue;
        Assert.True(isEntityDeletedProperty);
        Assert.Equal(createdDateBeforeRemove, entity.CreatedAt); // The created at does not change with the remove
        Assert.True(updatedDateBeforeRemove < entity.UpdatedAt);
        Assert.Equal(userIdOnCreate, entity.CreatedBy); // The created by does not change with the remove
        Assert.Equal(userIdOnRemove, entity.UpdatedBy);
        var fetchedRemovedChildEntity = _context.TestChildEntities
            .IgnoreQueryFilters()
            .FirstOrDefault(e => e.Parent.Id == entity.Id);
        Assert.NotNull(fetchedRemovedChildEntity);
        var isChildEntityDeletedProperty = _context.Entry(fetchedRemovedChildEntity)
            .Property<bool>(EntityProperties.IsDeleted)
            .CurrentValue;
        Assert.True(isChildEntityDeletedProperty);
        Assert.True(updatedDateBeforeRemove < fetchedRemovedChildEntity.UpdatedAt); // The child entity is updated on remove
        Assert.Equal(userIdOnCreate, fetchedRemovedChildEntity.CreatedBy);
        Assert.Equal(userIdOnRemove, fetchedRemovedChildEntity.UpdatedBy); // The child entity is updated on remove
    }

    public void Dispose()
    {
        _context.Dispose();
        _testScope.Dispose();
    }
}