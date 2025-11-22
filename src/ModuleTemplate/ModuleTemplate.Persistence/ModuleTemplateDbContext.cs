using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using SharedCore.Persistence;

namespace ModuleTemplate.Persistence;

/// <summary>
/// The ModuleTemplate DB context.
/// </summary>
/// <seealso cref="BaseDbContext{TContext}"/>
[ExcludeFromCodeCoverage]
internal class ModuleTemplateDbContext : BaseDbContext<ModuleTemplateDbContext>
{
    protected override string DefaultSchema => "ModuleTemplate";

    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleTemplateDbContext"/> class.
    /// </summary>
    /// <param name="options">The options.</param>
    public ModuleTemplateDbContext(DbContextOptions options) : base(options)
    {
    }
}