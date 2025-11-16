using System.Reflection;

namespace Architecture.Tests.Helpers;

/// <summary>
/// Patterns for specific namespaces.
/// </summary>
internal static class Namespaces
{
    public const string System = "System";

    public const string Microsoft = "Microsoft";

    public const string MicrosoftEfCore = "Microsoft.EntityFrameworkCore";

    public static readonly string[] Presentation = GetNamespacesFromAssemblies(Assemblies.Presentation);

    public static readonly string[] Application = GetNamespacesFromAssemblies(Assemblies.Application);

    public static readonly string[] Persistence = GetNamespacesFromAssemblies(Assemblies.Persistence);

    public static readonly string[] Domain = GetNamespacesFromAssemblies(Assemblies.Domain);

    public static readonly string[] Common = GetNamespacesFromAssemblies(Assemblies.Common);

    public const string SharedCoreStarting = "SharedCore.";

    public const string PatternForEndpointGroup = ".Presentation.Api.Endpoints";

    public const string PatternForCommandHandler = ".Application.Commands";

    public const string PatternForQueryHandler = ".Application.Queries";

    public const string PatternForRepository = ".Persistence.Repositories";

    public const string PatternForEntityTypeConfiguration = ".Persistence.Configurations";

    public const string PatternForEntities = ".Domain.Entities";

    private static string[] GetNamespacesFromAssemblies(IEnumerable<Assembly> assemblies)
    {
        return assemblies.Select(t => t.GetName().Name!).ToArray();
    }
}