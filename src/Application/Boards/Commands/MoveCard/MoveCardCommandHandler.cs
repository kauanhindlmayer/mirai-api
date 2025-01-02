using Application.Common.Interfaces.Persistence;
using Domain.Boards;
using ErrorOr;
using MediatR;

namespace Application.Boards.Commands.MoveCard;

internal sealed class MoveCardCommandHandler(IBoardsRepository boardRepository)
    : IRequestHandler<MoveCardCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        MoveCardCommand command,
        CancellationToken cancellationToken)
    {
        var board = await boardRepository.GetByIdWithCardsAsync(
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

        targetColumn.Cards.Insert(command.TargetPosition, card);
        boardRepository.Update(board);

        return Result.Success;
    }
}