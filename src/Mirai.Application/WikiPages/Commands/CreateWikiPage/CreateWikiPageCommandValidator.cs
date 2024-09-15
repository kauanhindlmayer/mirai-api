using FluentValidation;

namespace Mirai.Application.WikiPages.Commands.CreateWikiPage;

public class CreateWikiPageCommandValidator : AbstractValidator<CreateWikiPageCommand>
{
    public CreateWikiPageCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty();

        RuleFor(x => x.Title)
            .MinimumLength(3)
            .MaximumLength(255);

        RuleFor(x => x.Content)
            .NotEmpty();
    }
}