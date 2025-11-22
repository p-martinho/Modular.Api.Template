using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedCore.Application.Common.Models;
using SharedCore.Application.MappingExtensions;
using SharedCore.Application.Queries.Models;

namespace SharedCore.Application.Queries;

/// <summary>
/// The query handler (without input).
/// </summary>
/// <typeparam name="TOutData">The type of the output data.</typeparam>
/// <seealso cref="IQueryHandler{TIn,TOutData}"/>
public abstract class QueryHandler<TOutData> : IQueryHandler<TOutData>
{
    /// <summary>
    /// The logger.
    /// </summary>
    protected readonly ILogger Logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryHandler{TOutData}"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    protected QueryHandler(ILogger logger)
    {
        Logger = logger;
    }

    /// <inheritdoc />
    public async Task<QueryOut<TOutData>> HandleAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await ExecuteAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            return QueryOut<TOutData>.OperationCanceled();
        }
        catch (Exception e)
        {
            return HandleException(e);
        }
    }

    /// <summary>
    /// Executes the query and returns the query output asynchronous.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The query output.</returns>
    protected abstract Task<QueryOut<TOutData>> ExecuteAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Handles the exception.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <returns>The error query output.</returns>
    protected virtual QueryOut<TOutData> HandleException(Exception exception)
    {
        Logger.LogError(exception, "Error executing the query: {ExceptionMessage}", exception.Message);

        return QueryOut<TOutData>.InternalError();
    }
}

/// <summary>
/// The query handler.
/// </summary>
/// <typeparam name="TIn">The type of the input.</typeparam>
/// <typeparam name="TOutData">The type of the output data.</typeparam>
/// <seealso cref="IQueryHandler{TIn,TOutData}"/>
public abstract class QueryHandler<TIn, TOutData> : IQueryHandler<TIn, TOutData>
{
    private readonly IValidator<TIn>? _validator;

    /// <summary>
    /// The logger.
    /// </summary>
    protected readonly ILogger Logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryHandler{TIn,TOutData}"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="validator">The validator.</param>
    protected QueryHandler(ILogger logger,
        IValidator<TIn>? validator = null)
    {
        Logger = logger;
        _validator = validator;
    }

    /// <inheritdoc />
    public async Task<QueryOut<TOutData>> HandleAsync(TIn queryIn, CancellationToken cancellationToken = default)
    {
        try
        {
            var validationErrors = (await ValidateQueryInAsync(queryIn, cancellationToken)).ToList();

            if (validationErrors.Count != 0)
            {
                return HandleValidationErrors(validationErrors);
            }

            return await HandleQueryInAsync(queryIn, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            return QueryOut<TOutData>.OperationCanceled();
        }
        catch (Exception e)
        {
            return HandleException(queryIn, e);
        }
    }

    /// <summary>
    /// Validates the query input asynchronous.
    /// </summary>
    /// <param name="queryIn">The query input.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The validation errors (if any).</returns>
    protected virtual async Task<IEnumerable<ResultDetail>> ValidateQueryInAsync(TIn? queryIn,
        CancellationToken cancellationToken)
    {
        if (queryIn is null)
        {
            return [new ResultDetail("QueryIn", "The query input is required.")];
        }

        if (_validator is null)
        {
            return [];
        }

        var validationResult = await _validator.ValidateAsync(queryIn, cancellationToken);

        return validationResult.Errors.ToResultDetails();
    }

    /// <summary>
    /// Handles the validation error and returns the query output.
    /// </summary>
    /// <param name="validationErrors">The validations errors.</param>
    /// <returns>The validation error query output.</returns>
    protected virtual QueryOut<TOutData> HandleValidationErrors(IEnumerable<ResultDetail> validationErrors)
    {
        return QueryOut<TOutData>.ValidationError(resultDetails: validationErrors);
    }

    /// <summary>
    /// Handles the query input and returns the query output asynchronous.
    /// </summary>
    /// <param name="queryIn">The query input.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The query output.</returns>
    protected abstract Task<QueryOut<TOutData>> HandleQueryInAsync(TIn queryIn, CancellationToken cancellationToken);

    /// <summary>
    /// Handles the exception.
    /// </summary>
    /// <param name="queryIn">The query input.</param>
    /// <param name="exception">The exception.</param>
    /// <returns>The error query output.</returns>
    protected virtual QueryOut<TOutData> HandleException(TIn queryIn, Exception exception)
    {
        Logger.LogError(exception, "Error processing query input {@QueryIn}: {ExceptionMessage}",
            queryIn, exception.Message);

        return QueryOut<TOutData>.InternalError();
    }
}