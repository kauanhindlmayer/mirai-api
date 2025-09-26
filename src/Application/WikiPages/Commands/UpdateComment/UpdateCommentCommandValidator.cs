using FluentValidation;

namespace Application.WikiPages.Commands.UpdateComment;

internal sealed class UpdateCommentCommandValidator : AbstractValidator<UpdateCommentCommand>
{
    public UpdateCommentCommandValidator()
    {
        RuleFor(x => x.WikiPageId)
            .NotEmpty();

        RuleFor(x => x.CommentId)
            .NotEmpty();

        RuleFor(x => x.Content)
            .MinimumLength(3)
            .MaximumLength(255);
    }
}