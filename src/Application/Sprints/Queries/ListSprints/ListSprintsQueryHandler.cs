using Application.Abstractions;
using Domain.Sprints;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Sprints.Queries.ListSprints;

internal sealed class ListSprintsQueryHandler
    : IRequestHandler<ListSprintsQuery, ErrorOr<IReadOnlyList<SprintResponse>>>
{
    private readonly IApplicationDbContext _context;

    public ListSprintsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<IReadOnlyList<SprintResponse>>> Handle(
        ListSprintsQuery query,
        CancellationToken cancellationToken)
    {
        var sprints = await _context.Sprints
            .AsNoTracking()
            .Where(s => s.TeamId == query.TeamId)
            .OrderByDescending(s => s.EndDate)
            .Select(t => new SprintResponse
            {
                Id = t.Id,
                Name = t.Name,
                StartDate = t.StartDate,
                EndDate = t.EndDate,
                Status = t.Status,
                StartedAtUtc = t.StartedAtUtc,
                CommittedStoryPoints = t.Status == SprintStatus.Planned
                    ? null
                    : t.Commitments.Sum(c => c.CommittedStoryPoints ?? 0),
                CommittedWorkItemCount = t.Status == SprintStatus.Planned
                    ? null
                    : t.Commitments.Count,
                WorkItemCount = t.WorkItems.Count,
            })
            .ToListAsync(cancellationToken);

        return sprints;
    }
}