using FluentValidation;

namespace Application.Teams.Commands.RemoveUserFromTeam;

internal sealed class RemoveUserFromTeamCommandValidator
    : AbstractValidator<RemoveUserFromTeamCommand>
{
    public RemoveUserFromTeamCommandValidator()
    {
        RuleFor(x => x.TeamId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
