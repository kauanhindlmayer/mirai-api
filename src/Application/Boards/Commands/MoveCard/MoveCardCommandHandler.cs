using Domain.Boards;
using ErrorOr;
using MediatR;

namespace Application.Boards.Commands.MoveCard;

internal sealed class MoveCardCommandHandler
    : IRequestHandler<MoveCardCommand, ErrorOr<Success>>
{
    private readonly IBoardRepository _boardRepository;

    public MoveCardCommandHandler(IBoardRepository boardRepository)
    {
        _boardRepository = boardRepository;
    }

    public async Task<ErrorOr<Success>> Handle(
        MoveCardCommand command,
        CancellationToken cancellationToken)
    {
        var board = await _boardRepository.GetByIdWithCardsAsync(
            command.BoardId,
            cancellationToken);

        if (board is null)
        {
            return BoardErrors.NotFound;
        }

        var result = board.MoveCard(
            command.ColumnId,
            command.CardId,
            command.TargetColumnId,
            command.TargetPosition);

        if (result.IsError)
        {
            return result.Errors;
        }

        _boardRepository.Update(board);

        return Result.Success;
    }
}