using Application.Abstractions;
using Domain.Backlogs;
using Domain.Boards;
using Domain.WorkItems.Enums;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Boards.Queries.GetColumnCards;

internal sealed class GetColumnCardsQueryHandler
    : IRequestHandler<GetColumnCardsQuery, ErrorOr<ColumnCardsResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetColumnCardsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<ColumnCardsResponse>> Handle(
        GetColumnCardsQuery query,
        CancellationToken cancellationToken)
    {
        var column = await _context.BoardColumns
            .AsNoTracking()
            .Where(c => c.BoardId == query.BoardId && c.Id == query.ColumnId)
            .FirstOrDefaultAsync(cancellationToken);

        if (column is null)
        {
            return BoardErrors.NotFound;
        }

        var cardsQuery = _context.BoardCards
            .AsNoTracking()
            .Where(card => card.BoardColumnId == query.ColumnId);

        if (query.BacklogLevel.HasValue)
        {
            cardsQuery = query.BacklogLevel.Value switch
            {
                BacklogLevel.Epic => cardsQuery.Where(card =>
                    card.WorkItem.Type == WorkItemType.Epic && !card.WorkItem.ParentWorkItemId.HasValue),
                BacklogLevel.Feature => cardsQuery.Where(card => card.WorkItem.Type == WorkItemType.Feature),
                BacklogLevel.UserStory => cardsQuery.Where(card => card.WorkItem.Type == WorkItemType.UserStory),
                _ => cardsQuery,
            };
        }

        var totalCount = await cardsQuery.CountAsync(cancellationToken);

        var skip = (query.Page - 1) * query.PageSize;

        var cards = await cardsQuery
            .OrderBy(card => card.Position)
            .Skip(skip)
            .Take(query.PageSize)
            .Select(BoardQueries.ProjectToDto())
            .ToListAsync(cancellationToken);

        return new ColumnCardsResponse
        {
            Cards = cards,
            TotalCardCount = totalCount,
            HasMoreCards = totalCount > query.Page * query.PageSize,
        };
    }
}
