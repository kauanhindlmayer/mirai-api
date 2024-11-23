using FluentValidation;

namespace Application.Boards.Commands.DeleteBoard;

internal sealed class DeleteBoardCommandValidator : AbstractValidator<DeleteBoardCommand>
{
    public DeleteBoardCommandValidator()
    {
        RuleFor(x => x.BoardId)
            .NotEmpty();
    }
}