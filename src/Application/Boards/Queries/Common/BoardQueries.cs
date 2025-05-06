using System.Linq.Expressions;
using Application.Boards.Queries.GetBoard;
using Application.Boards.Queries.ListBoards;
using Domain.Boards;

namespace Application.Boards.Queries.Common;

internal static class BoardQueries
{
    private const int MaxCardsPerColumn = 20;

    public static Expression<Func<Board, BoardResponse>> ProjectToDto()
    {
        return b => new BoardResponse
        {
            Id = b.Id,
            TeamId = b.TeamId,
            Name = b.Name,
            Columns = b.Columns
                .OrderBy(column => column.Position)
                .Select(column => new BoardColumnResponse
                {
                    Id = column.Id,
                    Name = column.Name,
                    Position = column.Position,
                    WipLimit = column.WipLimit,
                    DefinitionOfDone = column.DefinitionOfDone,
                    Cards = column.Cards
                        .OrderBy(card => card.Position)
                        .Take(MaxCardsPerColumn)
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
                            CreatedAt = card.CreatedAt,
                            UpdatedAt = card.UpdatedAt,
                        }),
                }),
        };
    }

    public static Expression<Func<Board, BoardBriefResponse>> ProjectToBriefDto()
    {
        return b => new BoardBriefResponse
        {
            Id = b.Id,
            TeamId = b.TeamId,
            Name = b.Name,
        };
    }
}