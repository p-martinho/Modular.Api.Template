using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using SharedCore.Application.Common.Models;
using SharedCore.Application.Tests.TestHandlers;

namespace SharedCore.Application.Tests.Queries;

public class QueryHandlerTests
{
    private readonly TestQueryHandler _queryHandler;
    private readonly ILogger _logger;
    private readonly IValidator<string> _validator;

    public QueryHandlerTests()
    {
        _logger = Substitute.For<ILogger>();

        _validator = Substitute.For<IValidator<string>>();
        _validator.ValidateAsync(null!).ReturnsForAnyArgs(new ValidationResult());

        _queryHandler = new TestQueryHandler(_logger, _validator);
    }

    [Fact]
    public async Task HandleAsync_ShouldSucceed()
    {
        // Arrange

        // Act
        var result = await _queryHandler.HandleAsync("QueryIn", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.Success, result.Result.ResultType);
    }

    [Fact]
    public async Task HandleAsync_WhenValidationError_ShouldReturnValidationError()
    {
        // Arrange
        const string queryIn = "QueryIn";
        var validationFailures = new List<ValidationFailure>
        {
            new("Property1", "Message1"), new("Property2", "Message2")
        };
        _validator.ValidateAsync(queryIn, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult { Errors = validationFailures });

        // Act
        var result = await _queryHandler.HandleAsync(queryIn, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.ValidationError, result.Result.ResultType);
        Assert.NotNull(result.Result.ResultDetails);
        Assert.Equal(validationFailures.Count, result.Result.ResultDetails.Count());
        var firstResultDetail = result.Result.ResultDetails.First();
        var lastResultDetail = result.Result.ResultDetails.Last();
        var firstValidationFailure = validationFailures.First();
        var lastValidationFailure = validationFailures.Last();
        Assert.Equal(firstValidationFailure.PropertyName, firstResultDetail.Property);
        Assert.Equal(firstValidationFailure.ErrorMessage, firstResultDetail.Message);
        Assert.Equal(lastValidationFailure.PropertyName, lastResultDetail.Property);
        Assert.Equal(lastValidationFailure.ErrorMessage, lastResultDetail.Message);
    }

    [Fact]
    public async Task HandleAsync_WhenInputIsNull_ShouldReturnValidationError()
    {
        // Arrange
        const string queryIn = null!;

        // Act
        var result = await _queryHandler.HandleAsync(queryIn!, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.ValidationError, result.Result.ResultType);
        Assert.NotNull(result.Result.ResultDetails);
        Assert.Single(result.Result.ResultDetails);
        var resultDetail = result.Result.ResultDetails.Single();
        Assert.Equal("QueryIn", resultDetail.Property);
        Assert.Equal("The query input is required.", resultDetail.Message);
    }

    [Fact]
    public async Task HandleAsync_WhenValidatorIsNull_ShouldSucceed()
    {
        // Arrange
        var queryHandler = new TestQueryHandler(_logger, null!);

        // Act
        var result = await queryHandler.HandleAsync("QueryIn", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.Success, result.Result.ResultType);
    }

    [Fact]
    public async Task HandleAsync_WhenOperationCanceled_ShouldReturnOperationCanceled()
    {
        // Arrange
        _validator.ValidateAsync(null!, CancellationToken.None).ThrowsAsyncForAnyArgs<OperationCanceledException>();

        // Act
        var result = await _queryHandler.HandleAsync("QueryIn", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.OperationCanceled, result.Result.ResultType);
        _logger.DidNotReceiveWithAnyArgs().Log(Arg.Any<LogLevel>(), Arg.Any<EventId>(), Arg.Any<object>(),
            Arg.Any<Exception?>(), Arg.Any<Func<object, Exception?, string>>());
    }

    [Fact]
    public async Task HandleAsync_WhenException_ShouldLogAndReturnInternalError()
    {
        // Arrange
        _validator.ValidateAsync(null!, CancellationToken.None).ThrowsAsyncForAnyArgs<Exception>();

        // Act
        var result = await _queryHandler.HandleAsync("QueryIn", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.InternalError, result.Result.ResultType);
        _logger.Received().Log(LogLevel.Error, Arg.Any<EventId>(), Arg.Any<object>(),
            Arg.Is<Exception?>(e => e != null), Arg.Any<Func<object, Exception?, string>>());
    }
}