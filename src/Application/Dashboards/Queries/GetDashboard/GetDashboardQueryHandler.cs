using Application.Abstractions;
using Domain.Teams;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Dashboards.Queries.GetDashboard;

internal class GetDashboardQueryHandler
    : IRequestHandler<GetDashboardQuery, ErrorOr<DashboardResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IDashboardChartService _chartService;

    public GetDashboardQueryHandler(
        IApplicationDbContext context,
        IDashboardChartService chartService)
    {
        _context = context;
        _chartService = chartService;
    }

    public async Task<ErrorOr<DashboardResponse>> Handle(
        GetDashboardQuery query,
        CancellationToken cancellationToken)
    {
        if (!await _context.Teams.AnyAsync(p => p.Id == query.TeamId, cancellationToken))
        {
            return TeamErrors.NotFound;
        }

        var endDate = query.EndDate ?? DateTime.UtcNow;
        var startDate = query.StartDate ?? endDate.AddDays(-14);

        var burnChartTask = _chartService.GenerateBurnChartDataAsync(
            query.TeamId,
            startDate,
            endDate,
            cancellationToken);

        var leadTimeTask = _chartService.GenerateLeadTimeDataPointsAsync(
            query.TeamId,
            startDate,
            endDate,
            cancellationToken);

        var cycleTimeTask = _chartService.GenerateCycleTimeDataPointsAsync(
            query.TeamId,
            startDate,
            endDate,
            cancellationToken);

        var velocityTask = _chartService.GenerateVelocityDataPointsAsync(
            query.TeamId,
            cancellationToken);

        await Task.WhenAll(burnChartTask, leadTimeTask, cycleTimeTask, velocityTask);

        var (burnupData, burndownData) = await burnChartTask;
        var leadTimeData = await leadTimeTask;
        var cycleTimeData = await cycleTimeTask;
        var velocityData = await velocityTask;

        return new DashboardResponse(
            burnupData,
            burndownData,
            leadTimeData,
            cycleTimeData,
            velocityData,
            startDate,
            endDate);
    }
}
