using FluentValidation;

namespace Application.Organizations.Commands.UpdateOrganization;

internal sealed class UpdateOrganizationCommandValidator : AbstractValidator<UpdateOrganizationCommand>
{
    public UpdateOrganizationCommandValidator()
    {
        RuleFor(x => x.Name)
            .MinimumLength(3)
            .MaximumLength(255);

        RuleFor(x => x.Description)
            .NotNull()
            .MaximumLength(500);
    }
}