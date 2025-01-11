using Application.Common.Interfaces.Persistence;
using Domain.Boards;
using ErrorOr;
using MediatR;

namespace Application.Boards.Commands.MoveCard;

internal sealed class MoveCardCommandHandler : IRequestHandler<MoveCardCommand, ErrorOr<Success>>
{
    private readonly IBoardsRepository _boardsRepository;

    public MoveCardCommandHandler(IBoardsRepository boardsRepository)
    {
        _boardsRepository = boardsRepository;
    }

    public async Task<ErrorOr<Success>> Handle(
        MoveCardCommand command,
        CancellationToken cancellationToken)
    {
        var board = await _boardsRepository.GetByIdWithCardsAsync(
            command.BoardId,
            cancellationToken);

        if (board is null)
        {
            return BoardErrors.NotFound;
        }

        var column = board.Columns.FirstOrDefault(c => c.Id == command.ColumnId);
        if (column is null)
        {
            return BoardErrors.ColumnNotFound;
        }

        var card = column.Cards.First(c => c.Id == command.CardId);
        column.Cards.Remove(card);

        var targetColumn = board.Columns.FirstOrDefault(c => c.Id == command.TargetColumnId);
        if (targetColumn is null)
        {
            return BoardErrors.TargetColumnNotFound;
        }

        var result = targetColumn.AddCardAtPosition(card, command.TargetPosition);
        if (result.IsError)
        {
            return result.Errors;
        }

        _boardsRepository.Update(board);

        return Result.Success;
    }
}