using FluentValidation;

namespace Application.Boards.Commands.MoveCard;

public class MoveCardCommandValidator : AbstractValidator<MoveCardCommand>
{
    public MoveCardCommandValidator()
    {
        RuleFor(x => x.BoardId)
            .NotEmpty();

        RuleFor(x => x.ColumnId)
            .NotEmpty();

        RuleFor(x => x.CardId)
            .NotEmpty();

        RuleFor(x => x.TargetColumnId)
            .NotEmpty();

        RuleFor(x => x.TargetPosition)
            .GreaterThanOrEqualTo(0);
    }
}