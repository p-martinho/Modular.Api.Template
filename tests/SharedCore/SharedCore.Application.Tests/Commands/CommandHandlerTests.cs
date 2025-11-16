using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using SharedCore.Application.Common.Models;
using SharedCore.Application.Tests.TestHandlers;

namespace SharedCore.Application.Tests.Commands;

public class CommandHandlerTests
{
    private readonly TestCommandHandler _commandHandler;
    private readonly ILogger _logger;
    private readonly IValidator<string> _validator;

    public CommandHandlerTests()
    {
        _logger = Substitute.For<ILogger>();

        _validator = Substitute.For<IValidator<string>>();
        _validator.ValidateAsync(null!).ReturnsForAnyArgs(new ValidationResult());

        _commandHandler = new TestCommandHandler(_logger, _validator);
    }

    [Fact]
    public async Task HandleAsync_ShouldSucceed()
    {
        // Arrange

        // Act
        var result = await _commandHandler.HandleAsync("CommandIn", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.Success, result.Result.ResultType);
    }

    [Fact]
    public async Task HandleAsync_WhenValidationError_ShouldReturnValidationError()
    {
        // Arrange
        const string commandIn = "CommandIn";
        var validationFailures = new List<ValidationFailure>
        {
            new("Property1", "Message1"), new("Property2", "Message2")
        };
        _validator.ValidateAsync(commandIn, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult { Errors = validationFailures });

        // Act
        var result = await _commandHandler.HandleAsync(commandIn, TestContext.Current.CancellationToken);

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
        const string commandIn = null!;

        // Act
        var result = await _commandHandler.HandleAsync(commandIn!, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.ValidationError, result.Result.ResultType);
        Assert.NotNull(result.Result.ResultDetails);
        Assert.Single(result.Result.ResultDetails);
        var resultDetail = result.Result.ResultDetails.Single();
        Assert.Equal("CommandIn", resultDetail.Property);
        Assert.Equal("The command input is required.", resultDetail.Message);
    }

    [Fact]
    public async Task HandleAsync_WhenValidatorIsNull_ShouldSucceed()
    {
        // Arrange
        var commandHandler = new TestCommandHandler(_logger, null!);

        // Act
        var result = await commandHandler.HandleAsync("CommandIn", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.Success, result.Result.ResultType);
    }

    [Fact]
    public async Task HandleAsync_WhenOperationCanceled_ShouldReturnOperationCanceled()
    {
        // Arrange
        _validator.ValidateAsync(null!, CancellationToken.None).ThrowsAsyncForAnyArgs<OperationCanceledException>();

        // Act
        var result = await _commandHandler.HandleAsync("CommandIn", TestContext.Current.CancellationToken);

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
        var result = await _commandHandler.HandleAsync("CommandIn", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ResultType.InternalError, result.Result.ResultType);
        _logger.Received().Log(LogLevel.Error, Arg.Any<EventId>(), Arg.Any<object>(),
            Arg.Is<Exception?>(e => e != null), Arg.Any<Func<object, Exception?, string>>());
    }
}