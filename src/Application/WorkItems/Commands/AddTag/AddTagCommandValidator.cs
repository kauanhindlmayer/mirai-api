using FluentValidation;

namespace Application.WorkItems.Commands.AddTag;

public sealed class AddTagCommandValidator : AbstractValidator<AddTagCommand>
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