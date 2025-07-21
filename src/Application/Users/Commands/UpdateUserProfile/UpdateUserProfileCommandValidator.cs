using FluentValidation;

namespace Application.Users.Commands.UpdateUserProfile;

internal sealed class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(50)
            .Matches("^[a-zA-Z]+$").WithMessage("First name can only contain letters.");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(100)
            .Matches("^[a-zA-Z]+$").WithMessage("Last name can only contain letters.");
    }
}