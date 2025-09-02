using Application.Abstractions;
using Domain.Teams;
using Domain.WorkItems;
using Domain.WorkItems.Enums;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Dashboards.Queries.GetDashboard;

internal class GetDashboardQueryHandler
    : IRequestHandler<GetDashboardQuery, ErrorOr<DashboardResponse>>
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
        if (!await _context.Teams.AnyAsync(p => p.Id == query.TeamId, cancellationToken))
        {
            return TeamErrors.NotFound;
        }

        var workItems = await _context.WorkItems
            .Where(wi => wi.AssignedTeamId == query.TeamId)
            .ToListAsync(cancellationToken);

        var startDate = query.StartDate ?? (workItems.Count > 0
            ? workItems.Min(w => w.CreatedAtUtc)
            : DateTime.UtcNow.Date);
        var endDate = query.EndDate ?? DateTime.UtcNow;

        var burnupData = GenerateBurnupDataPoints(workItems, startDate, endDate);
        var burndownData = GenerateBurndownDataPoints(workItems, startDate, endDate);
        var leadTimeData = GenerateLeadTimeDataPoints(workItems, startDate, endDate);
        var cycleTimeData = GenerateCycleTimeDataPoints(workItems, startDate, endDate);
        var velocityData = await GenerateVelocityDataPoints(query.TeamId, cancellationToken);

        return new DashboardResponse(
            burnupData,
            burndownData,
            leadTimeData,
            cycleTimeData,
            velocityData,
            startDate,
            endDate);
    }

    private static List<BurndownPoint> GenerateBurndownDataPoints(
        List<WorkItem> workItems,
        DateTime startDate,
        DateTime endDate)
    {
        var dataPoints = new List<BurndownPoint>();

        var relevantItems = workItems
            .Where(wi => wi.CreatedAtUtc <= endDate)
            .ToList();

        var remainingWork = relevantItems.Count;

        for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
        {
            remainingWork -= relevantItems.Count(w => w.CompletedAtUtc?.Date == date);
            dataPoints.Add(new BurndownPoint(date, Math.Max(remainingWork, 0)));
        }

        return dataPoints;
    }

    private static List<BurnupPoint> GenerateBurnupDataPoints(
        List<WorkItem> workItems,
        DateTime startDate,
        DateTime endDate)
    {
        var dataPoints = new List<BurnupPoint>();

        var completedInRange = workItems
            .Where(wi => wi.CompletedAtUtc >= startDate && wi.CompletedAtUtc <= endDate)
            .ToList();

        var totalScope = workItems
            .Where(wi => wi.CreatedAtUtc <= endDate)
            .Count();

        int completedWork = 0;

        for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
        {
            completedWork += completedInRange.Count(w => w.CompletedAtUtc?.Date == date);
            dataPoints.Add(new BurnupPoint(date, completedWork, totalScope));
        }

        return dataPoints;
    }

    private static List<LeadTimePoint> GenerateLeadTimeDataPoints(
        List<WorkItem> workItems,
        DateTime startDate,
        DateTime endDate)
    {
        return workItems
            .Where(wi => wi.CompletedAtUtc.HasValue
                      && wi.CompletedAtUtc >= startDate
                      && wi.CompletedAtUtc <= endDate)
            .Select(wi => new LeadTimePoint(
                wi.CompletedAtUtc!.Value,
                (int)(wi.CompletedAtUtc!.Value - wi.CreatedAtUtc).TotalDays,
                wi.Title,
                wi.Type.ToString()))
            .OrderBy(lt => lt.CompletedDate)
            .ToList();
    }

    private static List<CycleTimePoint> GenerateCycleTimeDataPoints(
        List<WorkItem> workItems,
        DateTime startDate,
        DateTime endDate)
    {
        return workItems
            .Where(wi => wi.CompletedAtUtc.HasValue
                      && wi.CompletedAtUtc >= startDate
                      && wi.CompletedAtUtc <= endDate
                      && wi.Status == WorkItemStatus.Closed)
            .Select(wi => new CycleTimePoint(
                wi.CompletedAtUtc!.Value,
                CalculateCycleTime(wi),
                wi.Title,
                wi.Type.ToString()))
            .OrderBy(ct => ct.CompletedDate)
            .ToList();
    }

    private static int CalculateCycleTime(WorkItem workItem)
    {
        return (int)(workItem.CompletedAtUtc!.Value - workItem.CreatedAtUtc).TotalDays;
    }

    private async Task<List<VelocityPoint>> GenerateVelocityDataPoints(
        Guid teamId,
        CancellationToken cancellationToken)
    {
        var sprintVelocityData = await _context.Sprints
            .Where(s => s.TeamId == teamId)
            .OrderByDescending(s => s.StartDate)
            .Take(10)
            .Select(s => new
            {
                Sprint = s,
                CompletedWorkItems = s.WorkItems
                    .Where(wi => wi.CompletedAtUtc.HasValue
                              && (wi.Status == WorkItemStatus.Closed || wi.Status == WorkItemStatus.Resolved))
                    .ToList(),
            })
            .ToListAsync(cancellationToken);

        return sprintVelocityData
            .OrderBy(sv => sv.Sprint.StartDate)
            .Select(sv => new VelocityPoint(
                sv.Sprint.Name,
                sv.Sprint.StartDate.ToDateTime(TimeOnly.MinValue),
                sv.Sprint.EndDate.ToDateTime(TimeOnly.MaxValue),
                sv.CompletedWorkItems
                    .Where(wi => wi.Planning.StoryPoints.HasValue)
                    .Sum(wi => wi.Planning.StoryPoints!.Value),
                sv.CompletedWorkItems.Count))
            .ToList();
    }
}
