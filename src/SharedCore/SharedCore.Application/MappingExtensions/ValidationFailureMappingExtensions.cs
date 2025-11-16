using System.Diagnostics.CodeAnalysis;
using FluentValidation.Results;
using SharedCore.Application.Common.Models;

namespace SharedCore.Application.MappingExtensions;

/// <summary>
/// The validation failure mapping extensions.
/// </summary>
[ExcludeFromCodeCoverage]
public static class ValidationFailureMappingExtensions
{
    /// <summary>
    /// Converts the collection of validation failures into application result details.
    /// </summary>
    /// <param name="validationFailures">The collection of validation failures.</param>
    /// <returns>The collection of application result details.</returns>
    public static IEnumerable<ResultDetail> ToResultDetails(this IEnumerable<ValidationFailure> validationFailures)
    {
        return validationFailures.Select(validationFailure => validationFailure.ToResultDetail());
    }

    /// <summary>
    /// Converts the validation failure into an application result detail.
    /// </summary>
    /// <param name="validationFailure">The validation failure.</param>
    /// <returns>The application result detail.</returns>
    public static ResultDetail ToResultDetail(this ValidationFailure validationFailure)
    {
        return new ResultDetail(validationFailure.PropertyName, validationFailure.ErrorMessage);
    }
}