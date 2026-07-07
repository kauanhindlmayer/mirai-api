using FluentValidation;

namespace Application.WorkItems.Commands.RemovePullRequestLink;

internal sealed class RemovePullRequestLinkCommandValidator : AbstractValidator<RemovePullRequestLinkCommand>
{
    public RemovePullRequestLinkCommandValidator()
    {
        RuleFor(x => x.WorkItemId)
            .NotEmpty();

        RuleFor(x => x.LinkId)
            .NotEmpty();
    }
}
