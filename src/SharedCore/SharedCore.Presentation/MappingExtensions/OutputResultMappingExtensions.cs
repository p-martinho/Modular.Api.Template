using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedCore.Application.Common.Models;

namespace SharedCore.Presentation.MappingExtensions;

/// <summary>
/// The output result mapping extensions.
/// </summary>
[ExcludeFromCodeCoverage]
public static class OutputResultMappingExtensions
{
    private const string ResultDetailsExtensionKey = "resultDetails";

    /// <summary>
    /// The <see cref="OutputResult"/> extensions.
    /// </summary>
    /// <param name="result">The application output result.</param>
    extension(OutputResult result)
    {
        /// <summary>
        /// Converts the application output result into an API Problem Details.
        /// </summary>
        /// <returns>The API Problem Details.</returns>
        public ProblemDetails ToProblemDetails()
        {
            var problemDetails =
                new ProblemDetails { Status = result.ResultType.ToStatusCode(), Detail = result.Message };

            if (result.ResultDetails?.Any() == true)
            {
                problemDetails.Extensions.Add(ResultDetailsExtensionKey,
                    result.ResultDetails.Select(d => new { d.Property, d.Message }));
            }

            return problemDetails;
        }
    }

    /// <summary>
    /// The <see cref="ResultType"/> extensions.
    /// </summary>
    extension(ResultType resultType)
    {
        private int ToStatusCode()
        {
            return resultType switch
            {
                ResultType.Success => StatusCodes.Status200OK,
                ResultType.NotFoundError => StatusCodes.Status404NotFound,
                ResultType.ValidationError => StatusCodes.Status400BadRequest,
                ResultType.AuthorizationError => StatusCodes.Status403Forbidden,
                ResultType.InternalError => StatusCodes.Status500InternalServerError,
                ResultType.OperationCanceled => StatusCodes.Status422UnprocessableEntity,
                _ => throw new ArgumentOutOfRangeException(nameof(resultType), resultType,
                    "The result type is not supported.")
            };
        }
    }
}