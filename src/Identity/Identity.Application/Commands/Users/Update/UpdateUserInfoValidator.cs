using FluentValidation;
using Identity.Application.Dtos.Users.Update;

namespace Identity.Application.Commands.Users.Update;

/// <summary>
/// The validator for <see cref="UpdateUserInfoDto"/>.
/// </summary>
/// <seealso cref="AbstractValidator{T}"/>
internal class UpdateUserInfoValidator : AbstractValidator<UpdateUserInfoDto>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateUserInfoValidator"/> class.
    /// </summary>
    public UpdateUserInfoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => x.Email is not null);

        RuleFor(x => x.FirstName)
            .MaximumLength(64);

        RuleFor(x => x.LastName)
            .MaximumLength(64);
    }
}