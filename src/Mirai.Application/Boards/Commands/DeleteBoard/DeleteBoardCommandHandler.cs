using ErrorOr;
using MediatR;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.Boards;

namespace Mirai.Application.Boards.Commands.DeleteBoard;

public class DeleteBoardCommandHandler(IBoardsRepository _boardRepository)
    : IRequestHandler<DeleteBoardCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        DeleteBoardCommand request,
        CancellationToken cancellationToken)
    {
        var board = await _boardRepository.GetByIdAsync(
            request.BoardId,
            cancellationToken);

        if (board is null)
        {
            return BoardErrors.BoardNotFound;
        }

        _boardRepository.Remove(board);

        return Result.Success;
    }
}