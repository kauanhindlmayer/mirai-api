using FluentValidation;

namespace Application.Organizations.Commands.AddUserToOrganization;

internal sealed class AddUserToOrganizationCommandValidator
    : AbstractValidator<AddUserToOrganizationCommand>
{
    public AddUserToOrganizationCommandValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}