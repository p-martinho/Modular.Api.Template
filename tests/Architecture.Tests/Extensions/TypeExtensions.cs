using System.Runtime.CompilerServices;

namespace Architecture.Tests.Extensions;

internal static class TypeExtensions
{
    extension(Type t)
    {
        public bool IsNestedTypeForExtensionBlock()
        {
            return t.IsNested &&
                   t.DeclaringType is not null &&
                   t.DeclaringType.CustomAttributes.Any(c => c.AttributeType == typeof(ExtensionAttribute));
        }
    }
}