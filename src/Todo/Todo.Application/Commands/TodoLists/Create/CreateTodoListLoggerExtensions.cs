using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Todo.Application.Dtos.TodoLists.Create;

namespace Todo.Application.Commands.TodoLists.Create;

/// <summary>
/// The create to do list logger extensions.
/// </summary>
[ExcludeFromCodeCoverage]
internal static partial class CreateTodoListLoggerExtensions
{
    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "An unauthenticated attempt to create a to do list was performed with the input: {@CommandIn}")]
    public static partial void LogUnauthenticatedAttempt(this ILogger logger, CreateTodoListDto commandIn);
}