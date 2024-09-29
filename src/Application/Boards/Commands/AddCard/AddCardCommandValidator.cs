using FluentValidation;

namespace Application.Boards.Commands.AddCard;

public class AddCardCommandValidator : AbstractValidator<AddCardCommand>
{
    public AddCardCommandValidator()
    {
        RuleFor(x => x.BoardId)
            .NotEmpty();

        RuleFor(x => x.ColumnId)
            .NotEmpty();

        RuleFor(x => x.Type)
            .NotEmpty();

        RuleFor(x => x.Title)
            .MinimumLength(3)
            .MaximumLength(255);
    }
}