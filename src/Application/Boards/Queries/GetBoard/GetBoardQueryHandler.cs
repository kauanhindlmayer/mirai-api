using Application.Common.Interfaces;
using Domain.Boards;
using ErrorOr;
using MediatR;

namespace Application.Boards.Queries.GetBoard;

public class GetBoardQueryHandler(IBoardsRepository _boardRepository)
    : IRequestHandler<GetBoardQuery, ErrorOr<Board>>
{
    public async Task<ErrorOr<Board>> Handle(
        GetBoardQuery query,
        CancellationToken cancellationToken)
    {
        var board = await _boardRepository.GetByIdAsync(
            query.BoardId,
            cancellationToken);

        if (board is null)
        {
            return BoardErrors.NotFound;
        }

        return board;
    }
}