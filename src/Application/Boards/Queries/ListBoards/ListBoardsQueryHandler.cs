using Application.Abstractions;
using Application.Boards.Queries.Common;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Boards.Queries.ListBoards;

internal sealed class ListBoardsQueryHandler(IApplicationDbContext dbContext)
    : IRequestHandler<ListBoardsQuery, ErrorOr<IReadOnlyList<BoardBriefResponse>>>
{
    public async Task<ErrorOr<IReadOnlyList<BoardBriefResponse>>> Handle(
        ListBoardsQuery query,
        CancellationToken cancellationToken)
    {
        var boards = await dbContext.Boards
            .AsNoTracking()
            .Where(b => b.Team.ProjectId == query.ProjectId)
            .Select(BoardQueries.ProjectToBriefDto())
            .ToListAsync(cancellationToken);

        return boards;
    }
}