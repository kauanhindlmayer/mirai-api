using FluentValidation;

namespace Application.WorkItems.Commands.UnassignWorkItem;

internal sealed class UnassignWorkItemCommandValidator : AbstractValidator<UnassignWorkItemCommand>
{
    public UnassignWorkItemCommandValidator()
    {
        RuleFor(x => x.WorkItemId)
            .NotEmpty();
    }
}
