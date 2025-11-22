using NetArchTest.Rules;

namespace Architecture.Tests.Extensions;

internal static class PredicateExtensions
{
    extension(Predicate predicate)
    {
        public PredicateList DoNotResideInNamespaceStartingWith(string namespaceStart)
        {
            return predicate.DoNotResideInNamespaceMatching($"^{namespaceStart}");
        }
    }
}