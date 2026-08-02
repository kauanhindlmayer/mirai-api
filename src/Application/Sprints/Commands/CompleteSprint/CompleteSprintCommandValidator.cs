using Domain.Sprints;
using FluentValidation;

namespace Application.Sprints.Commands.CompleteSprint;

internal sealed class CompleteSprintCommandValidator : AbstractValidator<CompleteSprintCommand>
{
    public CompleteSprintCommandValidator()
    {
        RuleFor(x => x.TeamId)
            .NotEmpty();

        RuleFor(x => x.SprintId)
            .NotEmpty();

        RuleFor(x => x.Dispositions)
            .NotNull();

        RuleForEach(x => x.Dispositions)
            .ChildRules(disposition =>
            {
                disposition.RuleFor(d => d.WorkItemId)
                    .NotEmpty();

                disposition.RuleFor(d => d.Target)
                    .IsInEnum();

                disposition.RuleFor(d => d.TargetSprintId)
                    .NotEmpty()
                    .When(d => d.Target == DispositionTarget.Sprint);

                disposition.RuleFor(d => d.TargetSprintId)
                    .Null()
                    .When(d => d.Target == DispositionTarget.Backlog);
            });

        RuleFor(x => x.Dispositions)
            .Must(HaveDistinctWorkItems)
            .WithMessage("Each work item may have at most one disposition.");
    }

    private static bool HaveDistinctWorkItems(IReadOnlyList<SprintItemDisposition> dispositions)
    {
        return dispositions
            .Select(d => d.WorkItemId)
            .Distinct()
            .Count() == dispositions.Count;
    }
}
