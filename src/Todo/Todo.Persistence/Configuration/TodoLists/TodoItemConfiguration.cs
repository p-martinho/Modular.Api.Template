using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedCore.Persistence.Configurations;
using Todo.Domain.Entities.TodoLists;
using Todo.Domain.ValueObjects.TodoLists;

namespace Todo.Persistence.Configuration.TodoLists;

/// <summary>
/// The entity configuration for <see cref="TodoItem"/>.
/// </summary>
/// <seealso cref="BaseAuditableEntityConfiguration{TEntity}"/>
[ExcludeFromCodeCoverage]
internal class TodoItemConfiguration : BaseAuditableEntityConfiguration<TodoItem>
{
    public override void Configure(EntityTypeBuilder<TodoItem> builder)
    {
        base.Configure(builder);

        builder.ToTable("TodoItems");

        builder.Property(e => e.Title)
            .HasMaxLength(64);

        builder.Property(e => e.Description)
            .HasMaxLength(512);

        builder.OwnsOne(e => e.Schedule, s =>
        {
            s.Property(e => e.DueDate).HasColumnName(nameof(TodoItemSchedule.DueDate));
        });
    }
}