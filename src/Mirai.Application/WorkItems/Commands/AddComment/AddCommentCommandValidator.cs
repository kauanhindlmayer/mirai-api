using FluentValidation;

namespace Mirai.Application.WorkItems.Commands.AddComment;

public class AddCommentCommandValidator : AbstractValidator<AddCommentCommand>
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