using FluentValidation;

namespace Application.WikiPages.Commands.MoveWikiPage;

internal sealed class MoveWikiPageCommandValidator : AbstractValidator<MoveWikiPageCommand>
{
    public MoveWikiPageCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty();

        RuleFor(x => x.WikiPageId)
            .NotEmpty();

        RuleFor(x => x.TargetParentId)
            .NotEmpty();

        RuleFor(x => x.TargetPosition)
            .GreaterThanOrEqualTo(0);
    }
}