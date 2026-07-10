using FluentValidation;

namespace Application.Teams.Commands.ChangeTeamMemberRole;

internal sealed class ChangeTeamMemberRoleCommandValidator
    : AbstractValidator<ChangeTeamMemberRoleCommand>
{
    public ChangeTeamMemberRoleCommandValidator()
    {
        RuleFor(x => x.TeamId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.RoleId)
            .NotEmpty();
    }
}
