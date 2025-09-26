using Application.Abstractions;
using Application.Dashboards.Queries.GetDashboard;
using Domain.WorkItems.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Dashboards;

internal sealed class DashboardChartService : IDashboardChartService
{
    private readonly IApplicationDbContext _context;

    public DashboardChartService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(List<BurnupPoint> BurnupData, List<BurndownPoint> BurndownData)> GenerateBurnChartDataAsync(
        Guid teamId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        var totalScope = await _context.WorkItems
            .Where(wi => wi.AssignedTeamId == teamId && wi.CreatedAtUtc <= endDate)
            .CountAsync(cancellationToken);

        var completedByDate = await _context.WorkItems
            .Where(wi => wi.AssignedTeamId == teamId
                      && wi.CompletedAtUtc.HasValue
                      && wi.CompletedAtUtc >= startDate
                      && wi.CompletedAtUtc <= endDate)
            .GroupBy(wi => wi.CompletedAtUtc!.Value.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Date, x => x.Count, cancellationToken);

        var burnupPoints = new List<BurnupPoint>();
        var burndownPoints = new List<BurndownPoint>();
        var completedWork = 0;
        var remainingWork = totalScope;

        for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
        {
            var completedOnDate = completedByDate.GetValueOrDefault(date, 0);

            completedWork += completedOnDate;
            remainingWork -= completedOnDate;

            burnupPoints.Add(new BurnupPoint(date, completedWork, totalScope));
            burndownPoints.Add(new BurndownPoint(date, Math.Max(remainingWork, 0)));
        }

        return (burnupPoints, burndownPoints);
    }

    public async Task<List<LeadTimePoint>> GenerateLeadTimeDataPointsAsync(
        Guid teamId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        var workItems = await _context.WorkItems
            .Where(wi => wi.AssignedTeamId == teamId
                      && wi.CompletedAtUtc.HasValue
                      && wi.CompletedAtUtc >= startDate
                      && wi.CompletedAtUtc <= endDate)
            .Select(wi => new
            {
                wi.CompletedAtUtc,
                wi.CreatedAtUtc,
                wi.Title,
                WorkItemType = wi.Type.ToString(),
            })
            .OrderBy(wi => wi.CompletedAtUtc)
            .ToListAsync(cancellationToken);

        return workItems
            .Select(wi => new
            {
                wi,
                LeadTimeDays = (int)(wi.CompletedAtUtc!.Value - wi.CreatedAtUtc).TotalDays,
            })
            .Where(x => x.LeadTimeDays >= 0)
            .Select(x => new LeadTimePoint(
                x.wi.CompletedAtUtc!.Value,
                x.LeadTimeDays,
                x.wi.Title,
                x.wi.WorkItemType))
            .ToList();
    }

    public async Task<List<CycleTimePoint>> GenerateCycleTimeDataPointsAsync(
        Guid teamId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        var workItems = await _context.WorkItems
            .Where(wi => wi.AssignedTeamId == teamId
                      && wi.CompletedAtUtc.HasValue
                      && wi.CompletedAtUtc >= startDate
                      && wi.CompletedAtUtc <= endDate
                      && wi.Status == WorkItemStatus.Closed)
            .Select(wi => new
            {
                wi.CompletedAtUtc,
                wi.StartedAtUtc,
                wi.CreatedAtUtc,
                wi.Title,
                WorkItemType = wi.Type.ToString(),
            })
            .OrderBy(wi => wi.CompletedAtUtc)
            .ToListAsync(cancellationToken);

        return workItems
            .Select(wi => new
            {
                wi,
                CycleTimeDays = (int)(wi.CompletedAtUtc!.Value - (wi.StartedAtUtc ?? wi.CreatedAtUtc)).TotalDays,
            })
            .Where(x => x.CycleTimeDays >= 0)
            .Select(x => new CycleTimePoint(
                x.wi.CompletedAtUtc!.Value,
                x.CycleTimeDays,
                x.wi.Title,
                x.wi.WorkItemType))
            .ToList();
    }

    public async Task<List<VelocityPoint>> GenerateVelocityDataPointsAsync(
        Guid teamId,
        CancellationToken cancellationToken = default)
    {
        var sprints = await _context.Sprints
            .Include(s => s.WorkItems)
            .Where(s => s.TeamId == teamId)
            .OrderByDescending(s => s.StartDate)
            .Take(10)
            .Select(s => new
            {
                s.Name,
                s.StartDate,
                s.EndDate,
                WorkItems = s.WorkItems
                    .Where(wi => wi.CompletedAtUtc.HasValue
                              && (wi.Status == WorkItemStatus.Closed || wi.Status == WorkItemStatus.Resolved))
                    .Select(wi => new { wi.Planning.StoryPoints })
                    .ToList(),
            })
            .ToListAsync(cancellationToken);

        return sprints
            .Select(s => new VelocityPoint(
                s.Name,
                s.StartDate.ToDateTime(TimeOnly.MinValue),
                s.EndDate.ToDateTime(TimeOnly.MaxValue),
                s.WorkItems
                    .Where(wi => wi.StoryPoints.HasValue)
                    .Sum(wi => wi.StoryPoints!.Value),
                s.WorkItems.Count))
            .OrderBy(vp => vp.SprintStartDate)
            .ToList();
    }
}