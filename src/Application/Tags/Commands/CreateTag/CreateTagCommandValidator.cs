using Application.Tags.Commands.Common;
using FluentValidation;

namespace Application.Tags.Commands.CreateTag;

internal sealed class CreateTagCommandValidator : AbstractValidator<CreateTagCommand>
{
    public CreateTagCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty();

        RuleFor(x => x.Name)
            .MinimumLength(3)
            .MaximumLength(50);

        RuleFor(x => x.Description)
            .MaximumLength(500);

        RuleFor(x => x.Color)
            .MustBeAValidColor()
            .When(x => !string.IsNullOrWhiteSpace(x.Color));
    }
}