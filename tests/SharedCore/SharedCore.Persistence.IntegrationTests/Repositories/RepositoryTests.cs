using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NSubstitute;
using SharedCore.Persistence.IntegrationTests.Fixtures;
using SharedCore.Persistence.IntegrationTests.TestEntities;
using SharedCore.Persistence.IntegrationTests.TestRepositories;
using SharedCore.Persistence.Repositories.Settings;

namespace SharedCore.Persistence.IntegrationTests.Repositories;

public class RepositoryTests : BaseRepositoryIntegrationTest
{
    private readonly TestRepository _repository;

    public RepositoryTests(RepositoryFixture fixture) : base(fixture)
    {
        var queryParametersOptions = Substitute.For<IOptionsSnapshot<QueryParametersSettings>>();
        queryParametersOptions.Value.Returns(new QueryParametersSettings());

        _repository = new TestRepository(DbContext, queryParametersOptions);
    }

    [Fact]
    public async Task AddAsync_ShouldSucceed()
    {
        // Arrange
        var entity = new TestEntity { OwnerId = "OwnerId", Code = Guid.NewGuid().ToString() };

        // Act
        await _repository.AddAsync(entity, TestContext.Current.CancellationToken);

        // Assert
        Assert.Contains(DbContext.ChangeTracker.Entries<TestEntity>(), e => e.Entity == entity);
    }

    [Fact]
    public async Task AddRangeAsync_ShouldSucceed()
    {
        // Arrange
        IList<TestEntity> entities =
        [
            new() { OwnerId = "OwnerId", Code = Guid.NewGuid().ToString() },
            new() { OwnerId = "OwnerId", Code = Guid.NewGuid().ToString() }
        ];

        // Act
        await _repository.AddRangeAsync(entities, TestContext.Current.CancellationToken);

        // Assert
        foreach (var entity in entities)
        {
            Assert.Contains(DbContext.ChangeTracker.Entries<TestEntity>(), e => e.Entity == entity);
        }
    }

    [Fact]
    public async Task RemoveAsync_ShouldSucceed()
    {
        // Arrange
        var entity = new TestEntity { OwnerId = "OwnerId", Code = Guid.NewGuid().ToString() };
        await _repository.AddAsync(entity, TestContext.Current.CancellationToken);
        await _repository.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        await _repository.RemoveAsync(entity, TestContext.Current.CancellationToken);

        // Assert
        var entry = DbContext.Entry(entity);
        Assert.Equal(EntityState.Deleted, entry.State);
    }

    [Fact]
    public async Task RemoveRangeAsync_ShouldSucceed()
    {
        // Arrange
        IList<TestEntity> entities =
        [
            new() { OwnerId = "OwnerId", Code = Guid.NewGuid().ToString() },
            new() { OwnerId = "OwnerId", Code = Guid.NewGuid().ToString() }
        ];
        await _repository.AddRangeAsync(entities, TestContext.Current.CancellationToken);
        await _repository.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        await _repository.RemoveRangeAsync(entities, TestContext.Current.CancellationToken);

        // Assert
        foreach (var entity in entities)
        {
            var entry = DbContext.Entry(entity);
            Assert.Equal(EntityState.Deleted, entry.State);
        }
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldSucceed()
    {
        // Arrange
        var code = Guid.NewGuid().ToString();
        var entity = new TestEntity { OwnerId = "OwnerId", Code = code };
        await _repository.AddAsync(entity, TestContext.Current.CancellationToken);

        // Act
        await _repository.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Assert
        DbContext.ChangeTracker.Clear();
        var fetchedEntity =
            await _repository.FirstOrDefaultAsync(e => e.Code == code, TestContext.Current.CancellationToken);
        Assert.NotNull(fetchedEntity);
        Assert.NotSame(entity, fetchedEntity);
    }
}