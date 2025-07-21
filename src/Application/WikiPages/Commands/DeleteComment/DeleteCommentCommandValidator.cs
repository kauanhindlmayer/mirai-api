using FluentValidation;

namespace Application.WikiPages.Commands.DeleteComment;

internal sealed class DeleteCommentCommandValidator : AbstractValidator<DeleteCommentCommand>
{
    public DeleteCommentCommandValidator()
    {
        RuleFor(x => x.WikiPageId)
            .NotEmpty();

        RuleFor(x => x.CommentId)
            .NotEmpty();
    }
}