using FluentValidation;

namespace Application.WorkItems.Commands.UpdateComment;

internal sealed class UpdateCommentCommandValidator : AbstractValidator<UpdateCommentCommand>
{
    public UpdateCommentCommandValidator()
    {
        RuleFor(x => x.WorkItemId)
            .NotEmpty();

        RuleFor(x => x.CommentId)
            .NotEmpty();

        RuleFor(x => x.Content)
            .MinimumLength(3)
            .MaximumLength(255);
    }
}