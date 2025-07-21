using FluentValidation;

namespace Application.Sprints.Commands.CreateSprint;

internal sealed class CreateSprintCommandValidator : AbstractValidator<CreateSprintCommand>
{
    public CreateSprintCommandValidator()
    {
        RuleFor(x => x.TeamId)
            .NotEmpty();

        RuleFor(x => x.Name)
            .MinimumLength(3)
            .MaximumLength(50);

        RuleFor(x => x.StartDate)
            .NotEmpty();

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .GreaterThan(x => x.StartDate);
    }
}