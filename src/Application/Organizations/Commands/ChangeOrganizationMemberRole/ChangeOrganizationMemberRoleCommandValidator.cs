using FluentValidation;

namespace Application.Organizations.Commands.ChangeOrganizationMemberRole;

internal sealed class ChangeOrganizationMemberRoleCommandValidator
    : AbstractValidator<ChangeOrganizationMemberRoleCommand>
{
    public ChangeOrganizationMemberRoleCommandValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.RoleId)
            .NotEmpty();
    }
}
