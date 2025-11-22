using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedCore.Persistence.Configurations;
using Todo.Domain.Entities.TodoLists;

namespace Todo.Persistence.Configuration.TodoLists;

/// <summary>
/// The entity configuration for <see cref="TodoList"/>.
/// </summary>
/// <seealso cref="BaseOwnedEntityConfiguration{TEntity}"/>
[ExcludeFromCodeCoverage]
internal class TodoListConfiguration : BaseOwnedEntityConfiguration<TodoList>
{
    public override void Configure(EntityTypeBuilder<TodoList> builder)
    {
        base.Configure(builder);

        builder.ToTable("TodoLists");

        builder.Property(e => e.Name)
            .HasMaxLength(64);

        builder.HasMany(e => e.Items)
            .WithOne()
            .HasForeignKey(e => e.TodoListId);
    }
}