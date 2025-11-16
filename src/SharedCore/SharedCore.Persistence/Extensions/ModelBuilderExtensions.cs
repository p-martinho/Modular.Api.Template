using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SharedCore.Domain.Abstractions;
using SharedCore.Persistence.Constants;

namespace SharedCore.Persistence.Extensions;

/// <summary>
/// The model builder extensions.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class ModelBuilderExtensions
{
    /// <summary>
    /// Adds the soft delete property to the soft deletable entity types.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    /// <returns>The model builder.</returns>
    public static ModelBuilder AddSoftDeleteProperty(this ModelBuilder modelBuilder)
    {
        var softDeletableEntityTypes = modelBuilder.Model.GetEntityTypes()
            .Where(e => typeof(ISoftDeletableEntity).IsAssignableFrom(e.ClrType));

        foreach (var entityType in softDeletableEntityTypes)
        {
            entityType.AddProperty(EntityProperties.IsDeleted, typeof(bool));

            var clrType = entityType.ClrType;

            modelBuilder.Entity(clrType).HasQueryFilter(BuildIsNotDeletedExpression(clrType));
        }

        return modelBuilder;
    }

    private static LambdaExpression BuildIsNotDeletedExpression(Type entityType)
    {
        // Create parameter of provided type: 'e'
        var parameter = Expression.Parameter(entityType, "e");

        // Build method: 'EF.Property<bool>'
        var method = typeof(EF).GetMethod("Property")!.MakeGenericMethod(typeof(bool));

        // Build method call: 'EF.Property<bool>(e, IsDeletedProperty)'
        var methodCallExpression = Expression.Call(method, parameter, Expression.Constant(EntityProperties.IsDeleted));

        // Negate method call expression: '!EF.Property<bool>(e, IsDeletedProperty)'
        var notExpression = Expression.Not(methodCallExpression);

        return Expression.Lambda(notExpression, parameter);
    }
}