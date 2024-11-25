using FluentValidation;

namespace Application.Retrospectives.Commands.CreateColumn;

public sealed class CreateColumnCommandValidator : AbstractValidator<CreateColumnCommand>
{
    public CreateColumnCommandValidator()
    {
        RuleFor(x => x.Title)
            .MinimumLength(3)
            .MaximumLength(255);

        RuleFor(x => x.RetrospectiveId)
            .NotEmpty();
    }
}