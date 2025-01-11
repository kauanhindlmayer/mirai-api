using Application.Common.Interfaces.Persistence;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.WorkItems.Queries.GetWorkItemsStats;

internal sealed class GetWorkItemsStatsQueryHandler : IRequestHandler<GetWorkItemsStatsQuery, ErrorOr<WorkItemsStatsResponse>>
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

        var workItemsCreatedTask = _context.WorkItems
            .AsNoTracking()
            .Where(wi => wi.ProjectId == query.ProjectId && wi.CreatedAt >= startDate)
            .CountAsync(cancellationToken);

        var workItemsCompletedTask = _context.WorkItems
            .AsNoTracking()
            .Where(wi => wi.ProjectId == query.ProjectId && wi.CompletedAt >= startDate)
            .CountAsync(cancellationToken);

        await Task.WhenAll(workItemsCreatedTask, workItemsCompletedTask);

        return new WorkItemsStatsResponse
        {
            WorkItemsCreated = workItemsCreatedTask.Result,
            WorkItemsCompleted = workItemsCompletedTask.Result,
        };
    }
}