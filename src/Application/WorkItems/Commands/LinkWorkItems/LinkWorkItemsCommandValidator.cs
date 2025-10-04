using FluentValidation;

namespace Application.WorkItems.Commands.LinkWorkItems;

internal sealed class LinkWorkItemsCommandValidator : AbstractValidator<LinkWorkItemsCommand>
{
    public LinkWorkItemsCommandValidator()
    {
        RuleFor(x => x.SourceWorkItemId)
            .NotEmpty();

        RuleFor(x => x.TargetWorkItemId)
            .NotEmpty();

        RuleFor(x => x.LinkType)
            .IsInEnum();

        RuleFor(x => x.Comment)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Comment));
    }
}
