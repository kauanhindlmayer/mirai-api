using FluentValidation;

namespace Application.Users.Commands.UpdateUserProfile;

internal sealed class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(50)
            .Matches(@"^[\p{L}\p{M}\s'-]+$")
            .WithMessage("First name can only contain letters, spaces, hyphens, and apostrophes.");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(100)
            .Matches(@"^[\p{L}\p{M}\s'-]+$")
            .WithMessage("Last name can only contain letters, spaces, hyphens, and apostrophes.");
    }
}