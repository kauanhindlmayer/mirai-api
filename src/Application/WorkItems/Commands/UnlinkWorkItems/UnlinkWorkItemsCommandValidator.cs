using FluentValidation;

namespace Application.WorkItems.Commands.UnlinkWorkItems;

internal sealed class UnlinkWorkItemsCommandValidator : AbstractValidator<UnlinkWorkItemsCommand>
{
    public UnlinkWorkItemsCommandValidator()
    {
        RuleFor(x => x.WorkItemId)
            .NotEmpty();

        RuleFor(x => x.LinkId)
            .NotEmpty();
    }
}
