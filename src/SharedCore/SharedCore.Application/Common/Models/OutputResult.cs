using System.Diagnostics.CodeAnalysis;

namespace SharedCore.Application.Common.Models;

/// <summary>
/// The output result.
/// </summary>
/// <param name="ResultType">The output result type.</param>
/// <param name="Message">The output message.</param>
/// <param name="ResultDetails">The result details (optional).</param>
[ExcludeFromCodeCoverage]
public record OutputResult(
    ResultType ResultType,
    string? Message = null,
    IEnumerable<ResultDetail>? ResultDetails = null)
{
    /// <summary>
    /// Value indicating whether the result is successful.
    /// </summary>
    public bool IsSuccess => ResultType is ResultType.Success;
}