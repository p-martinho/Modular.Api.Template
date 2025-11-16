using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using SharedCore.Persistence;

namespace Todo.Persistence;

/// <summary>
/// The to do DB context.
/// </summary>
/// <seealso cref="BaseDbContext{TContext}"/>
[ExcludeFromCodeCoverage]
internal class TodoDbContext : BaseDbContext<TodoDbContext>
{
    protected override string DefaultSchema => "todo";

    /// <summary>
    /// Initializes a new instance of the <see cref="TodoDbContext"/> class.
    /// </summary>
    /// <param name="options">The options.</param>
    public TodoDbContext(DbContextOptions options) : base(options)
    {
    }
}