using Application.Common.Interfaces.Persistence;
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
            .Where(b => b.ProjectId == query.ProjectId)
            .Select(b => new BoardBriefResponse
            {
                Id = b.Id,
                Name = b.Name,
            })
            .ToListAsync(cancellationToken);

        return boards;
    }
}