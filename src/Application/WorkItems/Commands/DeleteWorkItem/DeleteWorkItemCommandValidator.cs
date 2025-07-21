using FluentValidation;

namespace Application.WorkItems.Commands.DeleteWorkItem;

internal sealed class DeleteWorkItemCommandValidator : AbstractValidator<DeleteWorkItemCommand>
{
    public DeleteWorkItemCommandValidator()
    {
        RuleFor(x => x.WorkItemId)
            .NotEmpty();
    }
}