using FluentValidation;
using Todo.Application.Dtos.TodoLists.TodoItems.Create;

namespace Todo.Application.Commands.TodoLists.TodoItems.Create;

/// <summary>
/// The validator for <see cref="CreateTodoItemDto"/>.
/// </summary>
/// <seealso cref="AbstractValidator{T}"/>
internal class CreateTodoItemValidator : AbstractValidator<CreateTodoItemDto>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateTodoItemValidator"/> class.
    /// </summary>
    public CreateTodoItemValidator()
    {
        RuleFor(x => x.ListId)
            .NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(64);

        RuleFor(x => x.Description)
            .MaximumLength(512);

        RuleFor(x => x.Schedule!)
            .SetValidator(new TodoItemScheduleValidator())
            .When(x => x.Schedule is not null);
    }
}