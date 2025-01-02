using Application.Common.Interfaces.Persistence;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Boards.Queries.ListBoards;

internal sealed class ListBoardsQueryHandler(IApplicationDbContext dbContext)
    : IRequestHandler<ListBoardsQuery, ErrorOr<IReadOnlyList<BoardSummaryResponse>>>
{
    public async Task<ErrorOr<IReadOnlyList<BoardSummaryResponse>>> Handle(
        ListBoardsQuery query,
        CancellationToken cancellationToken)
    {
        var boards = await dbContext.Boards
            .Where(b => b.ProjectId == query.ProjectId)
            .Select(b => new BoardSummaryResponse
            {
                Id = b.Id,
                Name = b.Name,
            })
            .ToListAsync(cancellationToken);

        return boards;
    }
}