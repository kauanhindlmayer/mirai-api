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

        var (burnupData, burndownData) = await _chartService.GenerateBurnChartDataAsync(
            query.TeamId,
            startDate,
            endDate,
            cancellationToken);

        var leadTimeData = await _chartService.GenerateLeadTimeDataPointsAsync(
            query.TeamId,
            startDate,
            endDate,
            cancellationToken);

        var cycleTimeData = await _chartService.GenerateCycleTimeDataPointsAsync(
            query.TeamId,
            startDate,
            endDate,
            cancellationToken);

        var velocityData = await _chartService.GenerateVelocityDataPointsAsync(
            query.TeamId,
            cancellationToken);

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
