using System.Diagnostics.CodeAnalysis;

namespace ModuleTemplate.Presentation.Api.OpenApi;

/// <summary>
/// The information about the API.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class ApiInfoDetails
{
    /// <summary>
    /// The title.
    /// </summary>
    public const string Title = "ModuleTemplate API";

    /// <summary>
    /// The description.
    /// </summary>
    public const string Description = "The ModuleTemplate API.";

    /// <summary>
    /// The contact details.
    /// </summary>
    public static class Contact
    {
        /// <summary>
        /// The contact name.
        /// </summary>
        public const string Name = "PMart";

        /// <summary>
        /// The contact email.
        /// </summary>
        public const string Email = "pmart@example.com";
    }

    /// <summary>
    /// The licence details.
    /// </summary>
    public static class Licence
    {
        /// <summary>
        /// The licence name.
        /// </summary>
        public const string Name = "MIT License";

        /// <summary>
        /// The licence URL.
        /// </summary>
        public const string Url = "https://opensource.org/licenses/MIT";
    }
}