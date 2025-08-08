using Application.Abstractions;
using Domain.Backlogs;
using Domain.WorkItems;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Backlogs.Queries.GetBacklog;

internal sealed class GetBacklogQueryHandler
    : IRequestHandler<GetBacklogQuery, ErrorOr<IReadOnlyList<BacklogResponse>>>
{
    private readonly IApplicationDbContext _context;

    public GetBacklogQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<IReadOnlyList<BacklogResponse>>> Handle(
        GetBacklogQuery query,
        CancellationToken cancellationToken)
    {
        var backlogResponse = await _context.WorkItems
            .AsNoTracking()
            .Include(wi => wi.ChildWorkItems)
            .Where(wi =>
                wi.AssignedTeamId == query.TeamId &&
                (!query.SprintId.HasValue || wi.SprintId == query.SprintId) &&
                (query.BacklogLevel == BacklogLevel.UserStory || !wi.ParentWorkItemId.HasValue) &&
                (!query.BacklogLevel.HasValue || query.BacklogLevel.Value.ToString() == wi.Type.ToString()))
            .Select(wi => ToDto(wi))
            .ToListAsync(cancellationToken);

        return backlogResponse;
    }

    private static BacklogResponse ToDto(WorkItem workItem)
    {
        return new BacklogResponse
        {
            Id = workItem.Id,
            Code = workItem.Code,
            Type = workItem.Type.ToString(),
            Title = workItem.Title,
            Status = workItem.Status.ToString(),
            StoryPoints = workItem.Planning.StoryPoints,
            ValueArea = workItem.Classification.ValueArea.ToString(),
            Tags = workItem.Tags.Select(t => t.Name),
            Children = workItem.ChildWorkItems.Select(ToDto),
        };
    }
}
