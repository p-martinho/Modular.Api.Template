using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using SharedCore.Application.Common.Models;
using SharedCore.Application.Dtos;
using SharedCore.Persistence.Repositories.Models;
using Todo.Application.Queries.TodoLists.Get;
using Todo.Domain.Entities.TodoLists;
using Todo.Persistence.Repositories.TodoLists;

namespace Todo.Application.Tests.Queries.TodoLists.Get;

public class GetTodoListsQueryHandlerTests
{
    private readonly GetTodoListsQueryHandler _queryHandler;
    private readonly ITodoListRepository _repository;

    public GetTodoListsQueryHandlerTests()
    {
        _repository = Substitute.For<ITodoListRepository>();

        _queryHandler = new GetTodoListsQueryHandler(
            NullLogger<GetTodoListsQueryHandler>.Instance,
            _repository);
    }

    [Fact]
    public async Task HandleAsync_ShouldSucceed()
    {
        // Arrange
        var queryIn = new PaginatedQueryDto { PageNumber = 1, PageSize = 10 };
        var todoLists = new List<TodoList> { TodoList.Create("OwnerId", "Name")! };
        var queryResult = new PaginatedQueryResult<TodoList>
        {
            Records = todoLists,
            Pagination = new QueryResultPagination
            {
                PageNumber = queryIn.PageNumber,
                PageSize = queryIn.PageSize,
                TotalRecords = todoLists.Count,
                PageRecords = todoLists.Count
            }
        };
        _repository.ListPaginatedAsync(Arg.Is<PaginatedQuerySpecification<TodoList>>(x =>
            x.PageNumber == queryIn.PageNumber &&
            x.PageSize == queryIn.PageSize &&
            x.Filters == null &&
            x.OrderBy == null &&
            x.IsToDisableEntityTracking &&
            !x.IsToDisableAggregateIncludes
        ), Arg.Any<CancellationToken>()).Returns(queryResult);

        // Act
        var result = await _queryHandler.HandleAsync(queryIn, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.Success, result.Result.ResultType);
        Assert.NotNull(result.Data);
        Assert.Equal(queryResult.Pagination.PageNumber, result.Data.Pagination.PageNumber);
        Assert.Equal(queryResult.Pagination.PageSize, result.Data.Pagination.PageSize);
        Assert.Equal(queryResult.Pagination.PageRecords, result.Data.Pagination.PageRecords);
        Assert.Equal(queryResult.Pagination.TotalRecords, result.Data.Pagination.TotalRecords);
        Assert.Equal(todoLists.Count, result.Data.Records.Count);
        Assert.Equal(todoLists.First().Name, result.Data.Records.First().Name);
    }

    [Fact]
    public async Task HandleAsync_WhenNoEntitiesFound_ShouldReturnEmptyCollection()
    {
        // Arrange
        var queryIn = new PaginatedQueryDto();
        var queryResult = new PaginatedQueryResult<TodoList>
        {
            Records = [],
            Pagination = new QueryResultPagination
            {
                PageNumber = 1,
                PageSize = 100,
                TotalRecords = 0,
                PageRecords = 0
            }
        };
        _repository.ListPaginatedAsync(null!, CancellationToken.None).ReturnsForAnyArgs(queryResult);

        // Act
        var result = await _queryHandler.HandleAsync(queryIn, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.Success, result.Result.ResultType);
        Assert.NotNull(result.Data);
        Assert.Equal(queryResult.Pagination.PageNumber, result.Data.Pagination.PageNumber);
        Assert.Equal(queryResult.Pagination.PageSize, result.Data.Pagination.PageSize);
        Assert.Equal(0, result.Data.Pagination.PageRecords);
        Assert.Equal(0, result.Data.Pagination.TotalRecords);
        Assert.Empty(result.Data.Records);
    }

    [Theory]
    [InlineData("Name", false)]
    [InlineData("Name", true)]
    public async Task HandleAsync_WhenOrderByIsSpecifiedAndSupported_ShouldApplySort(string orderBy,
        bool isDescendingOrder)
    {
        // Arrange
        var queryIn = new PaginatedQueryDto { OrderBy = orderBy, IsDescendingOrder = isDescendingOrder };
        var todoLists = new List<TodoList>
        {
            TodoList.Create("OwnerId", "Name3")!,
            TodoList.Create("OwnerId", "Name1")!,
            TodoList.Create("OwnerId", "Name2")!
        };
        var queryResult = new PaginatedQueryResult<TodoList> { Records = todoLists };
        var orderedList = new List<TodoList>();
        _repository.ListPaginatedAsync(Arg.Is<PaginatedQuerySpecification<TodoList>>(x => x.OrderBy != null),
                Arg.Any<CancellationToken>())
            .Returns(queryResult)
            .AndDoes(x =>
            {
                // this asserts that OrderBy works and adds code coverage
                var querySpecification = x[0] as PaginatedQuerySpecification<TodoList>;
                orderedList = querySpecification!.OrderBy!(queryResult.Records.AsQueryable()).ToList();
            });

        // Act
        var result = await _queryHandler.HandleAsync(queryIn, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.Success, result.Result.ResultType);
        Assert.NotNull(result.Data);
        Assert.Equal(todoLists.Count, result.Data.Records.Count);
        Assert.NotEmpty(orderedList);
        Assert.Equal(isDescendingOrder ? "Name3" : "Name1", orderedList.First().Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("SomeUnsupportedProperty")]
    public async Task HandleAsync_WhenOrderByIsNotSpecifiedOrNotSupported_ShouldNotApplySort(string? orderBy)
    {
        // Arrange
        var queryIn = new PaginatedQueryDto { OrderBy = orderBy };
        var todoLists = new List<TodoList> { TodoList.Create("OwnerId", "Name")! };
        var queryResult = new PaginatedQueryResult<TodoList> { Records = todoLists };
        _repository.ListPaginatedAsync(Arg.Is<PaginatedQuerySpecification<TodoList>>(x => x.OrderBy == null),
                Arg.Any<CancellationToken>())
            .Returns(queryResult);

        // Act
        var result = await _queryHandler.HandleAsync(queryIn, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.Success, result.Result.ResultType);
        Assert.NotNull(result.Data);
        Assert.Equal(todoLists.Count, result.Data.Records.Count);
    }

    [Theory]
    [InlineData("Name")]
    [InlineData("OwnerId")]
    public async Task HandleAsync_WhenFilterIsSpecifiedAndSupported_ShouldApplyFilter(string filterBy)
    {
        // Arrange
        var queryIn = new PaginatedQueryDto { FilterBy = filterBy, FilterValue = "SomeValue" };
        var todoLists = new List<TodoList> { TodoList.Create("OwnerId", "Name")! };
        var queryResult = new PaginatedQueryResult<TodoList> { Records = todoLists };
        _repository.ListPaginatedAsync(Arg.Is<PaginatedQuerySpecification<TodoList>>(x => x.Filters != null),
                Arg.Any<CancellationToken>())
            .Returns(queryResult);

        // Act
        var result = await _queryHandler.HandleAsync(queryIn, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.Success, result.Result.ResultType);
        Assert.NotNull(result.Data);
        Assert.Equal(todoLists.Count, result.Data.Records.Count);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("SomeUnsupportedProperty")]
    public async Task HandleAsync_WhenFilterByIsNotSpecifiedOrNotSupported_ShouldNotApplyFilter(string? filterBy)
    {
        // Arrange
        var queryIn = new PaginatedQueryDto { FilterBy = filterBy, FilterValue = "SomeValue" };
        var todoLists = new List<TodoList> { TodoList.Create("OwnerId", "Name")! };
        var queryResult = new PaginatedQueryResult<TodoList> { Records = todoLists };
        _repository.ListPaginatedAsync(Arg.Is<PaginatedQuerySpecification<TodoList>>(x => x.Filters == null),
                Arg.Any<CancellationToken>())
            .Returns(queryResult);

        // Act
        var result = await _queryHandler.HandleAsync(queryIn, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.Success, result.Result.ResultType);
        Assert.NotNull(result.Data);
        Assert.Equal(todoLists.Count, result.Data.Records.Count);
    }
}