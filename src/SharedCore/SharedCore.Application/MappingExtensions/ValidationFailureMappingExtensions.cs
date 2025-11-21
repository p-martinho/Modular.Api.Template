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
    /// The <see cref="IEnumerable{ValidationFailure}"/> extensions.
    /// </summary>
    /// <param name="validationFailures">The collection of validation failures.</param>
    extension(IEnumerable<ValidationFailure> validationFailures)
    {
        /// <summary>
        /// Converts the collection of validation failures into application result details.
        /// </summary>
        /// <returns>The collection of application result details.</returns>
        public IEnumerable<ResultDetail> ToResultDetails()
        {
            return validationFailures.Select(validationFailure => validationFailure.ToResultDetail());
        }
    }

    /// <summary>
    /// The <see cref="ValidationFailure"/> extensions.
    /// </summary>
    /// <param name="validationFailure">The validation failure.</param>
    extension(ValidationFailure validationFailure)
    {
        /// <summary>
        /// Converts the validation failure into an application result detail.
        /// </summary>
        /// <returns>The application result detail.</returns>
        public ResultDetail ToResultDetail()
        {
            return new ResultDetail(validationFailure.PropertyName, validationFailure.ErrorMessage);
        }
    }
}