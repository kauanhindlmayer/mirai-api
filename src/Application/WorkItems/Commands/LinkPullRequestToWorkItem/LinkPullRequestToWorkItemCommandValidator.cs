using FluentValidation;

namespace Application.WorkItems.Commands.LinkPullRequestToWorkItem;

internal sealed class LinkPullRequestToWorkItemCommandValidator : AbstractValidator<LinkPullRequestToWorkItemCommand>
{
    public LinkPullRequestToWorkItemCommandValidator()
    {
        RuleFor(x => x.WorkItemId)
            .NotEmpty();

        RuleFor(x => x.PullRequestNumber)
            .GreaterThan(0);
    }
}
