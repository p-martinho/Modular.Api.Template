namespace Todo.Presentation.Api;

/// <summary>
/// An interface marker to have a public type accessible to identify the assembly for this API, in external assembly.
/// </summary>
/// <remarks>the <see cref="Program"/> is internal, to be used it would require to add <c>public partial class Program { }</c> or set the internals visible to that assembly.</remarks>
public interface IApiMarker;