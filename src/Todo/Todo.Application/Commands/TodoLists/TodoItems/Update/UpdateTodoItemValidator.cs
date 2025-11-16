using FluentValidation;
using Todo.Application.Commands.TodoLists.TodoItems.Create;
using Todo.Application.Dtos.TodoLists.TodoItems.Create;
using Todo.Application.Dtos.TodoLists.TodoItems.Update;

namespace Todo.Application.Commands.TodoLists.TodoItems.Update;

/// <summary>
/// The validator for <see cref="CreateTodoItemDto"/>.
/// </summary>
/// <seealso cref="AbstractValidator{T}"/>
internal class UpdateTodoItemValidator : AbstractValidator<UpdateTodoItemDto>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateTodoItemValidator"/> class.
    /// </summary>
    public UpdateTodoItemValidator()
    {
        RuleFor(x => x.ListId)
            .NotEmpty();

        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(64)
            .When(x => x.Title is not null);

        RuleFor(x => x.Description)
            .MaximumLength(512)
            .When(x => x.Description is not null);

        RuleFor(x => x.Schedule!)
            .SetValidator(new TodoItemScheduleValidator())
            .When(x => x.Schedule is not null);
    }
}