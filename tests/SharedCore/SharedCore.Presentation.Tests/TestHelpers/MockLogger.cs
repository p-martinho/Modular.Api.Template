using Microsoft.Extensions.Logging;

namespace SharedCore.Presentation.Tests.TestHelpers;

/// <summary>
/// Abstract class to be used to mock an ILogger interface. It works with mocking frameworks like Moq and NSubstitute.
/// </summary>
/// <remarks>With a mock of the <see cref="ILogger" /> interface, to verify a call to Log, you need something like this:<br/>
/// <c>mockLogger.Verify(x => x.Log(
///     LogLevel.Information,
///     It.IsAny&lt;EventId>(),
///     It.Is&lt;It.IsAnyType>((o, t) => o.ToString().Contains("Index page say hello"))),
///     It.IsAny&lt;Exception>(),
///     It.IsAny&lt;Func&lt;It.IsAnyType, Exception?, string>>()),
///    Times.Once);</c><br/>
/// With this class, create a mock of it:<br/>
/// <c>mockLogger = new Mock&lt;MockLogger&lt;SomeService>>();</c><br/>
/// And then you can verify this way:<br/>
/// <c>mockLogger.Verify(x => x.LogInformation(It.Is&lt;string>(m => m.Contains("Index page say hello"))));</c>
/// </remarks>
/// <typeparam name="T">The type who's name is used for the logger category name.</typeparam>
/// <seealso cref="ILogger{T}"/>
internal abstract class MockLogger<T> : ILogger<T>
{
    void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (exception is null)
        {
            LogWithoutException(logLevel, formatter(state, exception));
        }
        else
        {
            LogWithException(logLevel, exception, formatter(state, exception));
        }
    }

    public virtual bool IsEnabled(LogLevel logLevel) => true;

    public abstract IDisposable BeginScope<TState>(TState state) where TState : notnull;

    public abstract void Log(LogLevel logLevel, string message);

    public abstract void Log(LogLevel logLevel, Exception exception, string message);

    public abstract void LogTrace(string message);

    public abstract void LogTrace(Exception exception, string message);

    public abstract void LogDebug(string message);

    public abstract void LogDebug(Exception exception, string message);

    public abstract void LogInformation(string message);

    public abstract void LogInformation(Exception exception, string message);

    public abstract void LogWarning(string message);

    public abstract void LogWarning(Exception exception, string message);

    public abstract void LogError(string message);

    public abstract void LogError(Exception exception, string message);

    public abstract void LogCritical(string message);

    public abstract void LogCritical(Exception exception, string message);

    private void LogWithoutException(LogLevel logLevel, string message)
    {
        switch (logLevel)
        {
            case LogLevel.Trace:
                LogTrace(message);
                break;

            case LogLevel.Debug:
                LogDebug(message);
                break;

            case LogLevel.Information:
                LogInformation(message);
                break;

            case LogLevel.Warning:
                LogWarning(message);
                break;

            case LogLevel.Error:
                LogError(message);
                break;

            case LogLevel.Critical:
                LogCritical(message);
                break;
        }

        Log(logLevel, message);
    }

    private void LogWithException(LogLevel logLevel, Exception exception, string message)
    {
        switch (logLevel)
        {
            case LogLevel.Trace:
                LogTrace(exception, message);
                break;

            case LogLevel.Debug:
                LogDebug(exception, message);
                break;

            case LogLevel.Information:
                LogInformation(exception, message);
                break;

            case LogLevel.Warning:
                LogWarning(exception, message);
                break;

            case LogLevel.Error:
                LogError(exception, message);
                break;

            case LogLevel.Critical:
                LogCritical(exception, message);
                break;
        }

        Log(logLevel, exception, message);
    }
}