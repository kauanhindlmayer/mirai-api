using FluentValidation;

namespace Mirai.Application.WikiPages.Commands.AddComment;

public class AddCommentCommandValidator : AbstractValidator<AddCommentCommand>
{
    public AddCommentCommandValidator()
    {
        RuleFor(x => x.WikiPageId)
            .NotEmpty();

        RuleFor(x => x.Content)
            .MinimumLength(3)
            .MaximumLength(255);
    }
}