using Application.Abstractions;
using Domain.Boards;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Boards.Queries.GetBoard;

internal sealed class GetBoardQueryHandler(IApplicationDbContext dbContext)
    : IRequestHandler<GetBoardQuery, ErrorOr<BoardResponse>>
{
    public async Task<ErrorOr<BoardResponse>> Handle(
        GetBoardQuery query,
        CancellationToken cancellationToken)
    {
        var board = await dbContext.Boards
            .AsNoTracking()
            .Where(b => b.Id == query.BoardId)
            .Select(BoardQueries.ProjectToDto(query.BacklogLevel))
            .FirstOrDefaultAsync(cancellationToken);

        if (board is null)
        {
            return BoardErrors.NotFound;
        }

        return board;
    }
}