using Application.Common.Interfaces.Persistence;
using Domain.Projects;
using Domain.WorkItems;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Dashboards.Queries.GetDashboard;

internal class GetDashboardQueryHandler : IRequestHandler<GetDashboardQuery, ErrorOr<DashboardResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetDashboardQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<DashboardResponse>> Handle(
        GetDashboardQuery query,
        CancellationToken cancellationToken)
    {
        if (!await _context.Projects.AnyAsync(p => p.Id == query.ProjectId, cancellationToken))
        {
            return ProjectErrors.NotFound;
        }

        var workItems = await _context.WorkItems
            .Where(wi => wi.ProjectId == query.ProjectId
                         && (query.StartDate == null || wi.CompletedAt >= query.StartDate)
                         && (query.EndDate == null || wi.CompletedAt <= query.EndDate))
            .ToListAsync(cancellationToken);

        var startDate = query.StartDate ?? workItems.Min(w => w.CreatedAt);
        var endDate = query.EndDate ?? DateTime.UtcNow;

        var burnupData = GenerateBurnupDataPoints(workItems, startDate, endDate);
        var burndownData = GenerateBurndownDataPoints(workItems, startDate, endDate);

        return new DashboardResponse(burnupData, burndownData, startDate, endDate);
    }

    private static List<BurndownPoint> GenerateBurndownDataPoints(
        List<WorkItem> workItems,
        DateTime startDate,
        DateTime endDate)
    {
        var dataPoints = new List<BurndownPoint>();
        var remainingWorkItems = workItems.Count;

        for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
        {
            remainingWorkItems -= workItems.Count(w => w.CompletedAt?.Date == date);
            dataPoints.Add(new BurndownPoint(date, Math.Max(remainingWorkItems, 0)));
        }

        return dataPoints;
    }

    private static List<BurnupPoint> GenerateBurnupDataPoints(
        List<WorkItem> workItems,
        DateTime startDate,
        DateTime endDate)
    {
        var dataPoints = new List<BurnupPoint>();
        var completedWorkItems = workItems.Where(wi => wi.CompletedAt != null).ToList();
        var totalWorkItems = workItems.Count;

        int completedWork = 0;

        for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
        {
            completedWork += completedWorkItems.Count(w => w.CompletedAt?.Date == date);
            dataPoints.Add(new BurnupPoint(date, completedWork, totalWorkItems));
        }

        return dataPoints;
    }
}