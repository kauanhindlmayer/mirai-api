using FluentValidation;

namespace Application.Boards.Commands.CreateColumn;

public sealed class CreateColumnCommandValidator : AbstractValidator<CreateColumnCommand>
{
    public CreateColumnCommandValidator()
    {
        RuleFor(x => x.Name)
            .MinimumLength(3)
            .MaximumLength(255);

        RuleFor(x => x.DefinitionOfDone)
            .MaximumLength(500);

        RuleFor(x => x.BoardId)
            .NotEmpty();
    }
}