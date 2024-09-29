using Application.Common.Interfaces;
using Domain.Boards;
using ErrorOr;
using MediatR;

namespace Application.Boards.Queries.GetBoard;

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