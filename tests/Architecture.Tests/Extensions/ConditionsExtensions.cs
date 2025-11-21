using NetArchTest.Rules;

namespace Architecture.Tests.Extensions;

internal static class ConditionsExtensions
{
    extension(Condition conditions)
    {
        public ConditionList ResideInAnyOfNamespaces(string[] namespaces)
        {
            if (namespaces.Length == 0)
            {
                throw new ArgumentException("Namespaces cannot be empty.", nameof(namespaces));
            }

            ConditionList conditionList = null!;

            foreach (var name in namespaces)
            {
                conditionList = conditionList is null
                    ? conditions.ResideInNamespace(name)
                    : conditionList.Or().ResideInNamespace(name);
            }

            return conditionList;
        }
    }
}