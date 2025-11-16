using NetArchTest.Rules;

namespace Architecture.Tests.Extensions;

internal static class PredicateExtensions
{
    public static PredicateList DoNotResideInNamespaceStartingWith(this Predicate predicate, string namespaceStart)
    {
        return predicate.DoNotResideInNamespaceMatching($"^{namespaceStart}");
    }
}