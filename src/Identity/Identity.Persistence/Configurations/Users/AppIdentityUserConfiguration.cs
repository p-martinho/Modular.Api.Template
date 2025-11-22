using System.Diagnostics.CodeAnalysis;
using Identity.Domain.Entities.Users;
using Identity.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Persistence.Configurations.Users;

/// <summary>
/// The entity configuration for <see cref="AppIdentityUser"/>.
/// </summary>
/// <seealso cref="IEntityTypeConfiguration{TEntity}"/>
[ExcludeFromCodeCoverage]
internal class AppIdentityUserConfiguration : IEntityTypeConfiguration<AppIdentityUser>
{
    public void Configure(EntityTypeBuilder<AppIdentityUser> builder)
    {
        builder.OwnsOne(e => e.Name, s =>
        {
            s.Property(e => e.FirstName)
                .HasColumnName(nameof(AppIdentityUserName.FirstName))
                .HasMaxLength(64);

            s.Property(e => e.LastName)
                .HasColumnName(nameof(AppIdentityUserName.LastName))
                .HasMaxLength(64);
        });
    }
}