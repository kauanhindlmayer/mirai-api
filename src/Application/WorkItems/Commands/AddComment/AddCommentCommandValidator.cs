using FluentValidation;

namespace Application.WorkItems.Commands.AddComment;

public sealed class AddCommentCommandValidator : AbstractValidator<AddCommentCommand>
{
    public AddCommentCommandValidator()
    {
        RuleFor(x => x.WorkItemId)
            .NotEmpty();

        RuleFor(x => x.Content)
            .MinimumLength(3)
            .MaximumLength(255);
    }
}