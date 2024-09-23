using FluentValidation;

namespace Mirai.Application.WorkItems.Commands.AddTag;

public class AddTagCommandValidator : AbstractValidator<AddTagCommand>
{
    public AddTagCommandValidator()
    {
        RuleFor(x => x.WorkItemId)
            .NotEmpty();

        RuleFor(x => x.TagName)
            .MinimumLength(3)
            .MaximumLength(50);
    }
}