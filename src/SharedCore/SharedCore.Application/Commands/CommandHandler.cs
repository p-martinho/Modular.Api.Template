using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedCore.Application.Commands.Models;
using SharedCore.Application.Common.Models;
using SharedCore.Application.MappingExtensions;

namespace SharedCore.Application.Commands;

/// <summary>
/// The abstract command handler (without input).
/// </summary>
/// <typeparam name="TOutData">The type of the output data.</typeparam>
/// <seealso cref="ICommandHandler{TOutData}"/>
public abstract class CommandHandler<TOutData> : ICommandHandler<TOutData>
{
    /// <summary>
    /// The logger.
    /// </summary>
    protected readonly ILogger Logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandHandler{TOutData}"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    protected CommandHandler(ILogger logger)
    {
        Logger = logger;
    }

    /// <inheritdoc />
    public async Task<CommandOut<TOutData>> HandleAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await ExecuteAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            return CommandOut<TOutData>.OperationCanceled();
        }
        catch (Exception e)
        {
            return HandleException(e);
        }
    }

    /// <summary>
    /// Executes the command and returns the command output asynchronous.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The command output.</returns>
    protected abstract Task<CommandOut<TOutData>> ExecuteAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Handles the exception.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <returns>The error command output.</returns>
    protected virtual CommandOut<TOutData> HandleException(Exception exception)
    {
        Logger.LogError(exception, "Error executing the command: {ExceptionMessage}", exception.Message);

        return CommandOut<TOutData>.InternalError();
    }
}

/// <summary>
/// The abstract command handler.
/// </summary>
/// <typeparam name="TIn">The type of the input.</typeparam>
/// <typeparam name="TOutData">The type of the output data.</typeparam>
/// <seealso cref="ICommandHandler{TIn,TOutData}"/>
public abstract class CommandHandler<TIn, TOutData> : ICommandHandler<TIn, TOutData>
{
    private readonly IValidator<TIn>? _validator;

    /// <summary>
    /// The logger.
    /// </summary>
    protected readonly ILogger Logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandHandler{TIn,TOutData}"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="validator">The validator.</param>
    protected CommandHandler(ILogger logger,
        IValidator<TIn>? validator = null)
    {
        Logger = logger;
        _validator = validator;
    }

    /// <inheritdoc />
    public async Task<CommandOut<TOutData>> HandleAsync(TIn commandIn, CancellationToken cancellationToken = default)
    {
        try
        {
            var validationErrors = (await ValidateCommandInAsync(commandIn, cancellationToken)).ToList();

            if (validationErrors.Count != 0)
            {
                return HandleValidationErrors(validationErrors);
            }

            return await HandleCommandInAsync(commandIn, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            return CommandOut<TOutData>.OperationCanceled();
        }
        catch (Exception e)
        {
            return HandleException(commandIn, e);
        }
    }

    /// <summary>
    /// Validates the command input asynchronous.
    /// </summary>
    /// <param name="commandIn">The command input.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The validation errors (if any).</returns>
    protected virtual async Task<IEnumerable<ResultDetail>> ValidateCommandInAsync(TIn? commandIn,
        CancellationToken cancellationToken)
    {
        if (commandIn is null)
        {
            return [new ResultDetail("CommandIn", "The command input is required.")];
        }

        if (_validator is null)
        {
            return [];
        }

        var validationResult = await _validator.ValidateAsync(commandIn, cancellationToken);

        return validationResult.Errors.ToResultDetails();
    }

    /// <summary>
    /// Handles the validation error and returns the command output.
    /// </summary>
    /// <param name="validationErrors">The validations errors.</param>
    /// <returns>The validation error command output.</returns>
    protected virtual CommandOut<TOutData> HandleValidationErrors(IEnumerable<ResultDetail> validationErrors)
    {
        return CommandOut<TOutData>.ValidationError(resultDetails: validationErrors);
    }

    /// <summary>
    /// Handles the command input and returns the command output asynchronous.
    /// </summary>
    /// <param name="commandIn">The command input.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The command output.</returns>
    protected abstract Task<CommandOut<TOutData>> HandleCommandInAsync(TIn commandIn,
        CancellationToken cancellationToken);

    /// <summary>
    /// Handles the exception.
    /// </summary>
    /// <param name="commandIn">The command input.</param>
    /// <param name="exception">The exception.</param>
    /// <returns>The error command output.</returns>
    protected virtual CommandOut<TOutData> HandleException(TIn commandIn, Exception exception)
    {
        Logger.LogError(exception, "Error processing command input {@CommandIn}: {ExceptionMessage}",
            commandIn, exception.Message);

        return CommandOut<TOutData>.InternalError();
    }
}