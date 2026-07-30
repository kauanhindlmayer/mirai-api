using Application.Abstractions;
using Domain.Sprints;
using Domain.Teams;
using Domain.WorkItems;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Sprints.Queries.GetIncompleteWorkItems;

internal sealed class GetIncompleteWorkItemsQueryHandler
    : IRequestHandler<GetIncompleteWorkItemsQuery, ErrorOr<IReadOnlyList<IncompleteWorkItemResponse>>>
{
    private readonly IApplicationDbContext _context;

    public GetIncompleteWorkItemsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<IReadOnlyList<IncompleteWorkItemResponse>>> Handle(
        GetIncompleteWorkItemsQuery query,
        CancellationToken cancellationToken)
    {
        var sprint = await _context.Sprints
            .AsNoTracking()
            .Include(s => s.WorkItems)
            .FirstOrDefaultAsync(s => s.Id == query.SprintId, cancellationToken);

        if (sprint is null)
        {
            return SprintErrors.NotFound;
        }

        if (sprint.TeamId != query.TeamId)
        {
            return TeamErrors.NotFound;
        }

        return sprint.IncompleteWorkItems()
            .Select(ToDto)
            .ToList();
    }

    private static IncompleteWorkItemResponse ToDto(WorkItem workItem)
    {
        return new IncompleteWorkItemResponse
        {
            Id = workItem.Id,
            Code = workItem.Code,
            Title = workItem.Title,
            Type = workItem.Type.ToString(),
            Status = workItem.Status.ToString(),
        };
    }
}