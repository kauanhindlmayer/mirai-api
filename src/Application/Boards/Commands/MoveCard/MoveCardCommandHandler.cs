using Application.Common.Interfaces;
using Domain.Boards;
using ErrorOr;
using MediatR;

namespace Application.Boards.Commands.MoveCard;

public class MoveCardCommandHandler(IBoardsRepository _boardRepository)
    : IRequestHandler<MoveCardCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        MoveCardCommand command,
        CancellationToken cancellationToken)
    {
        var board = await _boardRepository.GetByIdAsync(
            command.BoardId,
            cancellationToken);

        if (board is null)
        {
            return BoardErrors.BoardNotFound;
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

        targetColumn.Cards.Insert(command.TargetPosition, card);
        _boardRepository.Update(board);

        return Result.Success;
    }
}