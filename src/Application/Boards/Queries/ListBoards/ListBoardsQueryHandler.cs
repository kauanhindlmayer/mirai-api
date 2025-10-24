using Application.Abstractions;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Boards.Queries.ListBoards;

internal sealed class ListBoardsQueryHandler
    : IRequestHandler<ListBoardsQuery, ErrorOr<IReadOnlyList<BoardBriefResponse>>>
{
    private readonly IApplicationDbContext _context;

    public ListBoardsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<IReadOnlyList<BoardBriefResponse>>> Handle(
        ListBoardsQuery query,
        CancellationToken cancellationToken)
    {
        var boards = await _context.Boards
            .AsNoTracking()
            .Where(b => b.Team.ProjectId == query.ProjectId)
            .Select(BoardQueries.ProjectToBriefDto())
            .ToListAsync(cancellationToken);

        return boards;
    }
}