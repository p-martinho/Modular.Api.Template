using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using SharedCore.Application.Common.Models;
using SharedCore.Application.Tests.TestHandlers;

namespace SharedCore.Application.Tests.Queries;

public class QueryHandlerWithoutInputTests
{
    private readonly TestWithoutInputQueryHandler _queryHandler;
    private readonly ILogger _logger;
    private readonly ITestService _testService;

    public QueryHandlerWithoutInputTests()
    {
        _logger = Substitute.For<ILogger>();

        _testService = Substitute.For<ITestService>();

        _queryHandler = new TestWithoutInputQueryHandler(_logger, _testService);
    }

    [Fact]
    public async Task HandleAsync_ShouldSucceed()
    {
        // Arrange

        // Act
        var result = await _queryHandler.HandleAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.Success, result.Result.ResultType);
    }

    [Fact]
    public async Task HandleAsync_WhenOperationCanceled_ShouldReturnOperationCanceled()
    {
        // Arrange
        _testService.DoSomething().ThrowsForAnyArgs<OperationCanceledException>();

        // Act
        var result = await _queryHandler.HandleAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.OperationCanceled, result.Result.ResultType);
        _logger.DidNotReceiveWithAnyArgs().Log(Arg.Any<LogLevel>(), Arg.Any<EventId>(), Arg.Any<object>(),
            Arg.Any<Exception?>(), Arg.Any<Func<object, Exception?, string>>());
    }

    [Fact]
    public async Task HandleAsync_WhenException_ShouldLogAndReturnInternalError()
    {
        // Arrange
        _testService.DoSomething().ThrowsForAnyArgs<Exception>();

        // Act
        var result = await _queryHandler.HandleAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.InternalError, result.Result.ResultType);
        _logger.Received().Log(LogLevel.Error, Arg.Any<EventId>(), Arg.Any<object>(),
            Arg.Is<Exception?>(e => e != null), Arg.Any<Func<object, Exception?, string>>());
    }
}