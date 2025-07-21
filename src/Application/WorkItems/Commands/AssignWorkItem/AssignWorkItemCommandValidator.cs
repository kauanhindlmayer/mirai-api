using FluentValidation;

namespace Application.WorkItems.Commands.AssignWorkItem;

internal sealed class AssignWorkItemCommandValidator : AbstractValidator<AssignWorkItemCommand>
{
    public AssignWorkItemCommandValidator()
    {
        RuleFor(x => x.WorkItemId)
            .NotEmpty();

        RuleFor(x => x.AssigneeId)
            .NotEmpty();
    }
}