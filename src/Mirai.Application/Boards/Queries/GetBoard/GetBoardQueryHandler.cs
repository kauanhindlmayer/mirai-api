using ErrorOr;
using MediatR;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.Boards;

namespace Mirai.Application.Boards.Queries.GetBoard;

public class GetBoardQueryHandler(IBoardsRepository _boardRepository)
    : IRequestHandler<GetBoardQuery, ErrorOr<Board>>
{
    public async Task<ErrorOr<Board>> Handle(
        GetBoardQuery request,
        CancellationToken cancellationToken)
    {
        var board = await _boardRepository.GetByIdAsync(
            request.BoardId,
            cancellationToken);

        if (board is null)
        {
            return BoardErrors.BoardNotFound;
        }

        return board;
    }
}