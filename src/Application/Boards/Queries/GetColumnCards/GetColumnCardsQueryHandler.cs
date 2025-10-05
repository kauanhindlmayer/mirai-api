using Application.Abstractions;
using Application.Boards.Queries.GetBoard;
using Domain.Boards;
using Domain.WorkItems.Enums;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Boards.Queries.GetColumnCards;

internal sealed class GetColumnCardsQueryHandler
    : IRequestHandler<GetColumnCardsQuery, ErrorOr<ColumnCardsResponse>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetColumnCardsQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<ColumnCardsResponse>> Handle(
        GetColumnCardsQuery query,
        CancellationToken cancellationToken)
    {
        var column = await _dbContext.BoardColumns
            .AsNoTracking()
            .Where(c => c.BoardId == query.BoardId && c.Id == query.ColumnId)
            .FirstOrDefaultAsync(cancellationToken);

        if (column is null)
        {
            return BoardErrors.NotFound;
        }

        var cardsQuery = _dbContext.BoardCards
            .AsNoTracking()
            .Where(card => card.BoardColumnId == query.ColumnId);

        if (query.BacklogLevel.HasValue)
        {
            cardsQuery = query.BacklogLevel.Value switch
            {
                Domain.Backlogs.BacklogLevel.Epic => cardsQuery.Where(card =>
                    card.WorkItem.Type == WorkItemType.Epic && !card.WorkItem.ParentWorkItemId.HasValue),
                Domain.Backlogs.BacklogLevel.Feature => cardsQuery.Where(card =>
                    card.WorkItem.Type == WorkItemType.Feature),
                Domain.Backlogs.BacklogLevel.UserStory => cardsQuery.Where(card =>
                    card.WorkItem.Type == WorkItemType.UserStory),
                _ => cardsQuery,
            };
        }

        var totalCount = await cardsQuery.CountAsync(cancellationToken);

        var skip = (query.Page - 1) * query.PageSize;

        var cards = await cardsQuery
            .OrderBy(card => card.Position)
            .Skip(skip)
            .Take(query.PageSize)
            .Select(card => new BoardCardResponse
            {
                Id = card.Id,
                ColumnId = card.BoardColumnId,
                Position = card.Position,
                WorkItem = new WorkItemResponse
                {
                    Id = card.WorkItem.Id,
                    Code = card.WorkItem.Code,
                    Title = card.WorkItem.Title,
                    StoryPoints = card.WorkItem.Planning.StoryPoints,
                    Assignee = card.WorkItem.Assignee == null
                        ? null
                        : new AssigneeResponse
                        {
                            Id = card.WorkItem.Assignee.Id,
                            Name = card.WorkItem.Assignee.FullName,
                            ImageUrl = card.WorkItem.Assignee.ImageUrl,
                        },
                    Type = card.WorkItem.Type.ToString(),
                    Status = card.WorkItem.Status.ToString(),
                    Tags = card.WorkItem.Tags.Select(tag => tag.Name),
                },
                CreatedAtUtc = card.CreatedAtUtc,
                UpdatedAtUtc = card.UpdatedAtUtc,
            })
            .ToListAsync(cancellationToken);

        return new ColumnCardsResponse
        {
            Cards = cards,
            TotalCardCount = totalCount,
            HasMoreCards = totalCount > query.Page * query.PageSize,
        };
    }
}
