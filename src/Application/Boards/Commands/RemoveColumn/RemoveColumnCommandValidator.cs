using FluentValidation;

namespace Application.Boards.Commands.RemoveColumn;

public class RemoveColumnCommandValidator : AbstractValidator<RemoveColumnCommand>
{
    public RemoveColumnCommandValidator()
    {
        RuleFor(x => x.BoardId)
            .NotEmpty();

        RuleFor(x => x.ColumnId)
            .NotEmpty();
    }
}