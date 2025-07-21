using FluentValidation;

namespace Application.Teams.Commands.CreateTeam;

internal sealed class CreateTeamCommandValidator : AbstractValidator<CreateTeamCommand>
{
    public CreateTeamCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty();

        RuleFor(x => x.Name)
            .MinimumLength(3)
            .MaximumLength(255);
    }
}