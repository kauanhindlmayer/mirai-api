using FluentValidation;

namespace Application.Boards.Commands.AddColumn;

public class AddColumnCommandValidator : AbstractValidator<AddColumnCommand>
{
    public AddColumnCommandValidator()
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