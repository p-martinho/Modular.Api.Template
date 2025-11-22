using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity.Persistence;

/// <summary>
/// The identity DB context.
/// </summary>
/// <seealso cref="IdentityDbContext{TUser}"/>
[ExcludeFromCodeCoverage]
internal class IdentityDbContext : IdentityDbContext<IdentityUser>
{
    private const string? DefaultSchema = "identity";

    /// <summary>
    /// Initializes a new instance of the <see cref="IdentityDbContext"/> class.
    /// </summary>
    /// <param name="options">The options.</param>
    public IdentityDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseOpenIddict();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        if (!string.IsNullOrWhiteSpace(DefaultSchema))
        {
            modelBuilder.HasDefaultSchema(DefaultSchema);
        }

        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}