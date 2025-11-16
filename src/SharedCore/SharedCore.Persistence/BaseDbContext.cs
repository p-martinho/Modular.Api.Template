using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using SharedCore.Persistence.Extensions;

namespace SharedCore.Persistence;

/// <summary>
/// The base DB context.
/// </summary>
/// <typeparam name="TContext">The type of the specific BD context.</typeparam>
/// <seealso cref="DbContext"/>
[ExcludeFromCodeCoverage]
public abstract class BaseDbContext<TContext> : DbContext where TContext : BaseDbContext<TContext>
{
    /// <summary>
    /// The default schema to be used, when the schema is not set on the entity configuration.
    /// </summary>
    protected virtual string? DefaultSchema => null;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseDbContext{TContext}"/> class.
    /// </summary>
    /// <param name="options">The options.</param>
    protected BaseDbContext(DbContextOptions options) : base(options)
    {
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        if (!string.IsNullOrWhiteSpace(DefaultSchema))
        {
            modelBuilder.HasDefaultSchema(DefaultSchema);
        }

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TContext).Assembly);

        modelBuilder.AddSoftDeleteProperty();
    }
}