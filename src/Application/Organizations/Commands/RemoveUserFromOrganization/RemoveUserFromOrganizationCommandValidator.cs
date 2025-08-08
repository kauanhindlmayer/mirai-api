using FluentValidation;

namespace Application.Organizations.Commands.RemoveUserFromOrganization;

internal sealed class RemoveUserFromOrganizationCommandValidator
    : AbstractValidator<RemoveUserFromOrganizationCommand>
{
    public RemoveUserFromOrganizationCommandValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}