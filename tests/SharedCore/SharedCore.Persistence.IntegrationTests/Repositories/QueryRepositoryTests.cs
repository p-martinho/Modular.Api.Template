using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NSubstitute;
using SharedCore.Common.Authorization;
using SharedCore.Persistence.IntegrationTests.Fixtures;
using SharedCore.Persistence.IntegrationTests.TestEntities;
using SharedCore.Persistence.IntegrationTests.TestRepositories;
using SharedCore.Persistence.Repositories.Models;
using SharedCore.Persistence.Repositories.Settings;

namespace SharedCore.Persistence.IntegrationTests.Repositories;

public class QueryRepositoryTests : BaseRepositoryIntegrationTests
{
    private const int QueryMaxLimit = 10;

    private readonly TestQueryRepository _repository;

    public QueryRepositoryTests(RepositoryFixture fixture) : base(fixture)
    {
        var queryParametersOptions = Substitute.For<IOptionsSnapshot<QueryParametersSettings>>();
        queryParametersOptions.Value.Returns(new QueryParametersSettings { MaxLimit = QueryMaxLimit });

        _repository = new TestQueryRepository(DbContext, queryParametersOptions, CurrentUser);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, true)]
    public async Task GetByIdAsync_ShouldReturnEntity(bool isToDisableAggregateIncludes, bool isToDisableEntityTracking)
    {
        // Arrange
        var entityId = DatabaseSeeder.CurrentUserKnownTestEntityId;

        // Act
        var result = await _repository.GetByIdAsync(entityId,
            isToDisableAggregateIncludes: isToDisableAggregateIncludes,
            isToDisableEntityTracking: isToDisableEntityTracking,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);

        if (isToDisableAggregateIncludes)
        {
            Assert.Null(result.Children);
        }
        else
        {
            Assert.NotNull(result.Children);
        }

        if (isToDisableEntityTracking)
        {
            Assert.Empty(DbContext.ChangeTracker.Entries());
        }
        else
        {
            Assert.NotEmpty(DbContext.ChangeTracker.Entries());
        }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task GetByIdAsync_ShouldApplyPermissions(bool isAdmin)
    {
        // Arrange
        var entityId = DatabaseSeeder.OtherUserKnownTestEntityId;
        if (isAdmin)
        {
            CurrentUser.SetCurrentUserRole(UserRoles.Admin);
        }

        // Act
        var result = await _repository.GetByIdAsync(entityId, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        if (isAdmin)
        {
            Assert.NotNull(result);
        }
        else
        {
            Assert.Null(result);
        }
    }

    [Fact]
    public async Task FirstOrDefaultAsync_ShouldReturnEntity()
    {
        // Arrange
        const string code = "001";
        Expression<Func<TestEntity, bool>> filters = e => e.Code == code;

        // Act
        var result = await _repository.FirstOrDefaultAsync(filters, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(code, result.Code);
        Assert.NotNull(result.Children);
        Assert.NotEmpty(DbContext.ChangeTracker.Entries());
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task FirstOrDefaultAsync_ShouldApplyPermissions(bool isAdmin)
    {
        // Arrange
        const string code = "200";
        Expression<Func<TestEntity, bool>> filters = e => e.Code == code;
        if (isAdmin)
        {
            CurrentUser.SetCurrentUserRole(UserRoles.Admin);
        }

        // Act
        var result = await _repository.FirstOrDefaultAsync(filters, TestContext.Current.CancellationToken);

        // Assert
        if (isAdmin)
        {
            Assert.NotNull(result);
        }
        else
        {
            Assert.Null(result);
        }
    }

    [Fact]
    public async Task FirstOrDefaultAsync_ShouldApplyOrderById()
    {
        // Arrange
        Expression<Func<TestEntity, bool>> filters = _ => true;

        // Act
        var result = await _repository.FirstOrDefaultAsync(filters, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        var firstRecordOrderedById = await DbContext.TestEntities
            .Where(e => e.OwnerId == CurrentUser.UserId)
            .OrderBy(e => e.Id)
            .FirstAsync(TestContext.Current.CancellationToken);
        Assert.Equal(firstRecordOrderedById.Id, result.Id);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, true)]
    public async Task FirstOrDefaultAsync_WithQuerySpecification_ShouldReturnEntity(bool isToDisableAggregateIncludes,
        bool isToDisableEntityTracking)
    {
        // Arrange
        const string code = "001";
        var querySpecification = new QuerySpecification<TestEntity>
        {
            Filters = e => e.Code == code,
            IsToDisableAggregateIncludes = isToDisableAggregateIncludes,
            IsToDisableEntityTracking = isToDisableEntityTracking
        };

        // Act
        var result = await _repository.FirstOrDefaultAsync(querySpecification, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(code, result.Code);
        if (isToDisableAggregateIncludes)
        {
            Assert.Null(result.Children);
        }
        else
        {
            Assert.NotNull(result.Children);
        }

        if (isToDisableEntityTracking)
        {
            Assert.Empty(DbContext.ChangeTracker.Entries());
        }
        else
        {
            Assert.NotEmpty(DbContext.ChangeTracker.Entries());
        }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task FirstOrDefaultAsync_WithQuerySpecification_ShouldApplyPermissions(bool isAdmin)
    {
        // Arrange
        const string code = "200";
        var querySpecification = new QuerySpecification<TestEntity> { Filters = e => e.Code == code };
        if (isAdmin)
        {
            CurrentUser.SetCurrentUserRole(UserRoles.Admin);
        }

        // Act
        var result = await _repository.FirstOrDefaultAsync(querySpecification, TestContext.Current.CancellationToken);

        // Assert
        if (isAdmin)
        {
            Assert.NotNull(result);
        }
        else
        {
            Assert.Null(result);
        }
    }

    [Fact]
    public async Task FirstOrDefaultAsync_WithQuerySpecification_WhenOrderByIsSet_ShouldApplySort()
    {
        // Arrange
        var querySpecification = new QuerySpecification<TestEntity>
        {
            Filters = _ => true,
            OrderBy = q => q.OrderByDescending(e => e.Code)
        };

        // Act
        var result = await _repository.FirstOrDefaultAsync(querySpecification, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("100", result.Code);
    }

    [Fact]
    public async Task FirstOrDefaultAsync_WithQuerySpecification_WhenOrderByIsNotSet_ShouldApplyOrderById()
    {
        // Arrange
        var querySpecification = new QuerySpecification<TestEntity> { Filters = _ => true, OrderBy = null };

        // Act
        var result = await _repository.FirstOrDefaultAsync(querySpecification, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        var firstRecordOrderedById = await DbContext.TestEntities
            .Where(e => e.OwnerId == CurrentUser.UserId)
            .OrderBy(e => e.Id)
            .FirstAsync(TestContext.Current.CancellationToken);
        Assert.Equal(firstRecordOrderedById.Id, result.Id);
    }

    [Fact]
    public async Task SingleOrDefaultAsync_ShouldReturnEntity()
    {
        // Arrange
        const string code = "001";
        Expression<Func<TestEntity, bool>> filters = e => e.Code == code;

        // Act
        var result = await _repository.SingleOrDefaultAsync(filters, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(code, result.Code);
        Assert.NotNull(result.Children);
        Assert.NotEmpty(DbContext.ChangeTracker.Entries());
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task SingleOrDefaultAsync_ShouldApplyPermissions(bool isAdmin)
    {
        // Arrange
        const string code = "200";
        Expression<Func<TestEntity, bool>> filters = e => e.Code == code;
        if (isAdmin)
        {
            CurrentUser.SetCurrentUserRole(UserRoles.Admin);
        }

        // Act
        var result = await _repository.SingleOrDefaultAsync(filters, TestContext.Current.CancellationToken);

        // Assert
        if (isAdmin)
        {
            Assert.NotNull(result);
        }
        else
        {
            Assert.Null(result);
        }
    }

    [Fact]
    public async Task SingleOrDefaultAsync_WhenMoreThanOneEntityFound_ShouldThrowException()
    {
        // Arrange
        Expression<Func<TestEntity, bool>> filters = e => e.Code == "001" || e.Code == "002";

        // Act
        var result = () => _repository.SingleOrDefaultAsync(filters);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(result);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, true)]
    public async Task SingleOrDefaultAsync_WithQuerySpecification_ShouldReturnEntity(bool isToDisableAggregateIncludes,
        bool isToDisableEntityTracking)
    {
        // Arrange
        const string code = "001";
        var querySpecification = new QuerySpecification<TestEntity>
        {
            Filters = e => e.Code == code,
            IsToDisableAggregateIncludes = isToDisableAggregateIncludes,
            IsToDisableEntityTracking = isToDisableEntityTracking
        };

        // Act
        var result = await _repository.SingleOrDefaultAsync(querySpecification, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(code, result.Code);
        if (isToDisableAggregateIncludes)
        {
            Assert.Null(result.Children);
        }
        else
        {
            Assert.NotNull(result.Children);
        }

        if (isToDisableEntityTracking)
        {
            Assert.Empty(DbContext.ChangeTracker.Entries());
        }
        else
        {
            Assert.NotEmpty(DbContext.ChangeTracker.Entries());
        }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task SingleOrDefaultAsync_WithQuerySpecification_ShouldApplyPermissions(bool isAdmin)
    {
        // Arrange
        const string code = "200";
        var querySpecification = new QuerySpecification<TestEntity> { Filters = e => e.Code == code };
        if (isAdmin)
        {
            CurrentUser.SetCurrentUserRole(UserRoles.Admin);
        }

        // Act
        var result = await _repository.SingleOrDefaultAsync(querySpecification, TestContext.Current.CancellationToken);

        // Assert
        if (isAdmin)
        {
            Assert.NotNull(result);
        }
        else
        {
            Assert.Null(result);
        }
    }

    [Fact]
    public async Task SingleOrDefaultAsync_WithQuerySpecification_WhenMoreThanOneEntityFound_ShouldThrowException()
    {
        // Arrange
        var querySpecification = new QuerySpecification<TestEntity>
        {
            Filters = e => e.Code == "001" || e.Code == "002"
        };

        // Act
        var result = () => _repository.SingleOrDefaultAsync(querySpecification);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(result);
    }

    [Fact]
    public async Task ListAsync_ShouldReturnEntities()
    {
        // Arrange
        Expression<Func<TestEntity, bool>> filters = e => e.Code == "001" || e.Code == "002";

        // Act
        var result = (await _repository.ListAsync(filters, TestContext.Current.CancellationToken)).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result.Select(e => e.Children), Assert.NotNull);
        Assert.NotEmpty(DbContext.ChangeTracker.Entries());
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ListAsync_ShouldApplyPermissions(bool isAdmin)
    {
        // Arrange
        Expression<Func<TestEntity, bool>> filters = e => e.Code == "199" || e.Code == "200";
        if (isAdmin)
        {
            CurrentUser.SetCurrentUserRole(UserRoles.Admin);
        }

        // Act
        var result = await _repository.ListAsync(filters, TestContext.Current.CancellationToken);

        // Assert
        var expectedCount = isAdmin ? 2 : 0;
        Assert.Equal(expectedCount, result.Count());
    }

    [Fact]
    public async Task ListAsync_ShouldApplyQueryLimit()
    {
        // Arrange
        Expression<Func<TestEntity, bool>> filters = _ => true;

        // Act
        var result = await _repository.ListAsync(filters, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(QueryMaxLimit, result.Count());
    }

    [Fact]
    public async Task ListAsync_ShouldApplyOrderById()
    {
        // Arrange
        Expression<Func<TestEntity, bool>> filters = _ => true;

        // Act
        var result = await _repository.ListAsync(filters, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        var firstRecord = result.First();
        var firstRecordOrderedById = await DbContext.TestEntities
            .Where(e => e.OwnerId == CurrentUser.UserId)
            .OrderBy(e => e.Id)
            .FirstAsync(TestContext.Current.CancellationToken);
        Assert.Equal(firstRecordOrderedById.Id, firstRecord.Id);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, true)]
    public async Task ListAsync_WithQuerySpecification_ShouldReturnEntities(bool isToDisableAggregateIncludes,
        bool isToDisableEntityTracking)
    {
        // Arrange
        var querySpecification = new QuerySpecification<TestEntity>
        {
            Filters = e => e.Code == "001" || e.Code == "002",
            IsToDisableAggregateIncludes = isToDisableAggregateIncludes,
            IsToDisableEntityTracking = isToDisableEntityTracking
        };

        // Act
        var result = (await _repository.ListAsync(querySpecification, TestContext.Current.CancellationToken)).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        if (isToDisableAggregateIncludes)
        {
            Assert.All(result.Select(e => e.Children), Assert.Null);
        }
        else
        {
            Assert.All(result.Select(e => e.Children), Assert.NotNull);
        }

        if (isToDisableEntityTracking)
        {
            Assert.Empty(DbContext.ChangeTracker.Entries());
        }
        else
        {
            Assert.NotEmpty(DbContext.ChangeTracker.Entries());
        }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ListAsync_WithQuerySpecification_ShouldApplyPermissions(bool isAdmin)
    {
        // Arrange
        var querySpecification = new QuerySpecification<TestEntity>
        {
            Filters = e => e.Code == "199" || e.Code == "200"
        };
        if (isAdmin)
        {
            CurrentUser.SetCurrentUserRole(UserRoles.Admin);
        }

        // Act
        var result = await _repository.ListAsync(querySpecification, TestContext.Current.CancellationToken);

        // Assert
        var expectedCount = isAdmin ? 2 : 0;
        Assert.Equal(expectedCount, result.Count());
    }

    [Fact]
    public async Task ListAsync_WithQuerySpecification_ShouldApplyQueryLimit()
    {
        // Arrange
        var querySpecification = new QuerySpecification<TestEntity> { Filters = _ => true };

        // Act
        var result = await _repository.ListAsync(querySpecification, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(QueryMaxLimit, result.Count());
    }

    [Fact]
    public async Task ListAsync_WithQuerySpecification_WhenOrderByIsSet_ShouldApplySort()
    {
        // Arrange
        var querySpecification = new QuerySpecification<TestEntity>
        {
            Filters = _ => true,
            OrderBy = q => q.OrderByDescending(e => e.Code)
        };

        // Act
        var result = await _repository.ListAsync(querySpecification, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        var firstRecord = result.First();
        Assert.Equal("100", firstRecord.Code);
    }

    [Fact]
    public async Task ListAsync_WithQuerySpecification_WhenOrderByIsNotSet_ShouldApplyOrderById()
    {
        // Arrange
        var querySpecification = new QuerySpecification<TestEntity> { Filters = _ => true, OrderBy = null };

        // Act
        var result = await _repository.ListAsync(querySpecification, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        var firstRecord = result.First();
        var firstRecordOrderedById = await DbContext.TestEntities
            .Where(e => e.OwnerId == CurrentUser.UserId)
            .OrderBy(e => e.Id)
            .FirstAsync(TestContext.Current.CancellationToken);
        Assert.Equal(firstRecordOrderedById.Id, firstRecord.Id);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, true)]
    public async Task ListPaginatedAsync_ShouldReturnEntities(bool isToDisableAggregateIncludes,
        bool isToDisableEntityTracking)
    {
        // Arrange
        var querySpecification = new PaginatedQuerySpecification<TestEntity>
        {
            Filters = e => e.Code == "001" || e.Code == "002",
            IsToDisableAggregateIncludes = isToDisableAggregateIncludes,
            IsToDisableEntityTracking = isToDisableEntityTracking
        };

        // Act
        var result = await _repository.ListPaginatedAsync(querySpecification, TestContext.Current.CancellationToken);

        // Assert
        var records = result.Records.ToList();
        Assert.Equal(2, records.Count);
        Assert.Equal(2, result.Pagination.TotalRecords);
        if (isToDisableAggregateIncludes)
        {
            Assert.All(records.Select(e => e.Children), Assert.Null);
        }
        else
        {
            Assert.All(records.Select(e => e.Children), Assert.NotNull);
        }

        if (isToDisableEntityTracking)
        {
            Assert.Empty(DbContext.ChangeTracker.Entries());
        }
        else
        {
            Assert.NotEmpty(DbContext.ChangeTracker.Entries());
        }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ListPaginatedAsync_ShouldApplyPermissions(bool isAdmin)
    {
        // Arrange
        var querySpecification = new PaginatedQuerySpecification<TestEntity>
        {
            Filters = e => e.Code == "199" || e.Code == "200"
        };
        if (isAdmin)
        {
            CurrentUser.SetCurrentUserRole(UserRoles.Admin);
        }

        // Act
        var result = await _repository.ListPaginatedAsync(querySpecification, TestContext.Current.CancellationToken);

        // Assert
        var expectedCount = isAdmin ? 2 : 0;
        Assert.Equal(expectedCount, result.Records.Count());
        Assert.Equal(expectedCount, result.Pagination.TotalRecords);
    }

    [Fact]
    public async Task ListPaginatedAsync_ShouldApplyQueryLimit()
    {
        // Arrange
        var querySpecification = new PaginatedQuerySpecification<TestEntity> { Filters = _ => true };

        // Act
        var result = await _repository.ListPaginatedAsync(querySpecification, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(QueryMaxLimit, result.Records.Count());
        Assert.Equal(DatabaseSeeder.NumberOfRecordsOfUser, result.Pagination.TotalRecords);
    }

    [Fact]
    public async Task ListPaginatedAsync_WhenOrderByIsSet_ShouldApplySort()
    {
        // Arrange
        var querySpecification = new PaginatedQuerySpecification<TestEntity>
        {
            Filters = _ => true,
            OrderBy = q => q.OrderByDescending(e => e.Code),
            PageNumber = 1,
            PageSize = 5
        };

        // Act
        var result = await _repository.ListPaginatedAsync(querySpecification, TestContext.Current.CancellationToken);

        // Assert
        var expectedOrderedCodes = result.Records.OrderByDescending(e => e.Code).Select(e => e.Code).ToList();
        var codes = result.Records.Select(e => e.Code).ToList();
        Assert.Equal(expectedOrderedCodes, codes);
        Assert.Equal("100", codes.First());
        Assert.Equal("096", codes.Last());
    }

    [Fact]
    public async Task ListPaginatedAsync_WhenOrderByIsNotSet_ShouldApplyOrderById()
    {
        // Arrange
        var querySpecification = new PaginatedQuerySpecification<TestEntity>
        {
            Filters = _ => true,
            OrderBy = null,
            PageNumber = 1,
            PageSize = 5
        };

        // Act
        var result = await _repository.ListPaginatedAsync(querySpecification, TestContext.Current.CancellationToken);

        // Assert
        var firstRecord = result.Records.FirstOrDefault();
        Assert.NotNull(firstRecord);
        var firstRecordOrderedById = await DbContext.TestEntities
            .Where(e => e.OwnerId == CurrentUser.UserId)
            .OrderBy(e => e.Id)
            .FirstAsync(TestContext.Current.CancellationToken);
        Assert.Equal(firstRecordOrderedById.Id, firstRecord.Id);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(1, 5)]
    [InlineData(2, 5)]
    public async Task ListPaginatedAsync_ShouldApplyPagination(int pageNumber, int pageSize)
    {
        // Arrange
        var querySpecification = new PaginatedQuerySpecification<TestEntity>
        {
            Filters = _ => true,
            PageNumber = pageNumber,
            PageSize = pageSize,
            OrderBy = q => q.OrderBy(e => e.Code)
        };

        // Act
        var result = await _repository.ListPaginatedAsync(querySpecification, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(querySpecification.PageNumber, result.Pagination.PageNumber);
        Assert.Equal(querySpecification.PageSize, result.Pagination.PageSize);
        Assert.Equal(pageSize, result.Records.Count());
        Assert.Equal(result.Records.Count(), result.Pagination.PageRecords);
        var recordsOrderedByCode = result.Records.OrderBy(e => e.Code).ToList();
        var firstRecord = recordsOrderedByCode.First();
        var lastRecord = recordsOrderedByCode.Last();
        var firstCode = int.Parse(firstRecord.Code);
        var lastCode = int.Parse(lastRecord.Code);
        Assert.True(firstCode > (pageNumber - 1) * pageSize);
        Assert.True(lastCode <= pageNumber * pageSize);
        Assert.Equal(DatabaseSeeder.NumberOfRecordsOfUser, result.Pagination.TotalRecords);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task ListPaginatedAsync_WhenInvalidPageNumber_ShouldReturnFirstPage(int pageNumber)
    {
        // Arrange
        const int pageSize = 5;
        var querySpecification = new PaginatedQuerySpecification<TestEntity>
        {
            Filters = _ => true,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        // Act
        var result = await _repository.ListPaginatedAsync(querySpecification, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result.Pagination);
        Assert.Equal(pageSize, result.Pagination.PageSize);
        Assert.Equal(1, result.Pagination.PageNumber);
        Assert.Equal(pageSize, result.Records.Count());
        Assert.Equal(DatabaseSeeder.NumberOfRecordsOfUser, result.Pagination.TotalRecords);
    }

    [Fact]
    public async Task ListPaginatedAsync_WhenPageNumberIsHigherThanTotalPages_ShouldReturnNoRecords()
    {
        // Arrange
        const int pageSize = 5;
        const int totalNumberOfPages = DatabaseSeeder.NumberOfRecordsOfUser / pageSize;
        var querySpecification = new PaginatedQuerySpecification<TestEntity>
        {
            Filters = _ => true,
            PageNumber = totalNumberOfPages + 1,
            PageSize = pageSize
        };

        // Act
        var result = await _repository.ListPaginatedAsync(querySpecification, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(querySpecification.PageNumber, result.Pagination.PageNumber);
        Assert.Equal(querySpecification.PageSize, result.Pagination.PageSize);
        Assert.Empty(result.Records);
        Assert.Equal(DatabaseSeeder.NumberOfRecordsOfUser, result.Pagination.TotalRecords);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(QueryMaxLimit + 1)]
    public async Task ListPaginatedAsync_WhenPageSizeIsInvalid_ShouldApplyQueryLimit(int pageSize)
    {
        // Arrange
        var querySpecification = new PaginatedQuerySpecification<TestEntity>
        {
            Filters = _ => true,
            PageNumber = 1,
            PageSize = pageSize
        };

        // Act
        var result = await _repository.ListPaginatedAsync(querySpecification, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result.Pagination);
        Assert.Equal(querySpecification.PageNumber, result.Pagination.PageNumber);
        Assert.Equal(QueryMaxLimit, result.Pagination.PageSize);
        Assert.Equal(QueryMaxLimit, result.Records.Count());
        Assert.Equal(DatabaseSeeder.NumberOfRecordsOfUser, result.Pagination.TotalRecords);
    }

    [Fact]
    public async Task CountAsync_ShouldReturnCount()
    {
        // Arrange

        // Act
        var result = await _repository.CountAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(100, result);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task CountAsync_ShouldApplyPermissions(bool isAdmin)
    {
        // Arrange
        if (isAdmin)
        {
            CurrentUser.SetCurrentUserRole(UserRoles.Admin);
        }

        // Act
        var result = await _repository.CountAsync(TestContext.Current.CancellationToken);

        // Assert
        var expectedCount = isAdmin ? 200 : 100;
        Assert.Equal(expectedCount, result);
    }

    [Fact]
    public async Task CountAsync_WithFilters_ShouldReturnCount()
    {
        // Arrange
        Expression<Func<TestEntity, bool>> filters = e => e.Code == "001" || e.Code == "002";

        // Act
        var result = await _repository.CountAsync(filters, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(2, result);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task CountAsync_WithFilters_ShouldApplyPermissions(bool isAdmin)
    {
        // Arrange
        Expression<Func<TestEntity, bool>> filters = e => e.Code == "199" || e.Code == "200";
        if (isAdmin)
        {
            CurrentUser.SetCurrentUserRole(UserRoles.Admin);
        }

        // Act
        var result = await _repository.CountAsync(filters, TestContext.Current.CancellationToken);

        // Assert
        var expectedCount = isAdmin ? 2 : 0;
        Assert.Equal(expectedCount, result);
    }

    [Fact]
    public async Task AnyAsync_WhenAny_ShouldReturnTrue()
    {
        // Arrange

        // Act
        var result = await _repository.AnyAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task AnyAsync_WithFilters_WhenAny_ShouldReturnTrue()
    {
        // Arrange
        Expression<Func<TestEntity, bool>> filters = e => e.Code == "001";

        // Act
        var result = await _repository.AnyAsync(filters, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task AnyAsync_WithFilters_WhenNotAny_ShouldReturnFalse()
    {
        // Arrange
        Expression<Func<TestEntity, bool>> filters = e => e.Code == "unknownCode";

        // Act
        var result = await _repository.AnyAsync(filters, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task AnyAsync_WithFilters_ShouldApplyPermissions(bool isAdmin)
    {
        // Arrange
        Expression<Func<TestEntity, bool>> filters = e => e.Code == "200";
        if (isAdmin)
        {
            CurrentUser.SetCurrentUserRole(UserRoles.Admin);
        }

        // Act
        var result = await _repository.AnyAsync(filters, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(isAdmin, result);
    }
}