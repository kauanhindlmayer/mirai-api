using FluentValidation;

namespace Application.WikiPages.Commands.UpdateWikiPage;

public sealed class UpdateWikiPageCommandValidator : AbstractValidator<UpdateWikiPageCommand>
{
    public UpdateWikiPageCommandValidator()
    {
        RuleFor(x => x.WikiPageId)
            .NotEmpty();

        RuleFor(x => x.Title)
            .MinimumLength(3)
            .MaximumLength(255);

        RuleFor(x => x.Content)
            .NotEmpty();
    }
}