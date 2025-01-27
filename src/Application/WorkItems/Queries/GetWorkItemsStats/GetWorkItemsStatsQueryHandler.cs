using Application.Common.Interfaces.Persistence;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.WorkItems.Queries.GetWorkItemsStats;

internal sealed class GetWorkItemsStatsQueryHandler
    : IRequestHandler<GetWorkItemsStatsQuery, ErrorOr<WorkItemsStatsResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetWorkItemsStatsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<WorkItemsStatsResponse>> Handle(
        GetWorkItemsStatsQuery query,
        CancellationToken cancellationToken)
    {
        var startDate = DateTime.UtcNow.Date.AddDays(-query.PeriodInDays);

        var workItemsCreated = await _context.WorkItems
            .AsNoTracking()
            .Where(wi => wi.ProjectId == query.ProjectId && wi.CreatedAt >= startDate)
            .CountAsync(cancellationToken);

        var workItemsCompleted = await _context.WorkItems
            .AsNoTracking()
            .Where(wi => wi.ProjectId == query.ProjectId && wi.CompletedAt >= startDate)
            .CountAsync(cancellationToken);

        return new WorkItemsStatsResponse
        {
            WorkItemsCreated = workItemsCreated,
            WorkItemsCompleted = workItemsCompleted,
        };
    }
}