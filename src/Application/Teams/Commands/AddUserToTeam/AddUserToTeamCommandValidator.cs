using FluentValidation;

namespace Application.Teams.Commands.AddUserToTeam;

internal sealed class AddUserToTeamCommandValidator : AbstractValidator<AddUserToTeamCommand>
{
    public AddUserToTeamCommandValidator()
    {
        RuleFor(x => x.TeamId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}