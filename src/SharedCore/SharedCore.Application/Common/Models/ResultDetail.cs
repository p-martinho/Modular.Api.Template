using System.Diagnostics.CodeAnalysis;

namespace SharedCore.Application.Common.Models;

/// <summary>
/// The result detail.
/// </summary>
/// <param name="Property">The property to which the detail is related to.</param>
/// <param name="Message">The message.</param>
[ExcludeFromCodeCoverage]
public record ResultDetail(string Property, string Message);