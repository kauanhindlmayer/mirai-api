using FluentValidation;

namespace Application.Projects.Commands.ChangeProjectMemberRole;

internal sealed class ChangeProjectMemberRoleCommandValidator
    : AbstractValidator<ChangeProjectMemberRoleCommand>
{
    public ChangeProjectMemberRoleCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.RoleId)
            .NotEmpty();
    }
}
