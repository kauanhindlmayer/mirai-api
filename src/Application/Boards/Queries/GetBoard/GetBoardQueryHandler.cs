using Application.Common.Interfaces.Persistence;
using Domain.Boards;
using Domain.Users;
using Domain.WorkItems;
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
            .Where(b => b.Id == query.BoardId)
            .Select(board => new BoardResponse
            {
                Id = board.Id,
                ProjectId = board.ProjectId,
                Name = board.Name,
                Description = board.Description,
                Columns = board.Columns
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
                            .Select(card => new BoardCardResponse
                            {
                                Id = card.Id,
                                ColumnId = card.BoardColumnId,
                                Position = card.Position,
                                WorkItem = ToDto(card.WorkItem),
                                CreatedAt = card.CreatedAt,
                                UpdatedAt = card.UpdatedAt,
                            }),
                    }),
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (board is null)
        {
            return BoardErrors.NotFound;
        }

        return board;
    }

    private static WorkItemResponse ToDto(WorkItem workItem)
    {
        return new WorkItemResponse
        {
            Id = workItem.Id,
            Code = workItem.Code,
            Title = workItem.Title,
            StoryPoints = workItem.Planning.StoryPoints,
            Assignee = ToDto(workItem.Assignee),
            Type = workItem.Type.ToString(),
            Status = workItem.Status.ToString(),
            Tags = workItem.Tags.Select(tag => tag.Name),
        };
    }

    private static AssigneeResponse? ToDto(User? assignee)
    {
        return assignee is null
            ? null
            : new AssigneeResponse
            {
                Id = assignee.Id,
                Name = assignee.FullName,
                ImageUrl = assignee.ImageUrl,
            };
    }
}