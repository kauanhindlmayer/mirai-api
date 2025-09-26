using Application.Dashboards.Queries.GetDashboard;

namespace Application.Abstractions;

public interface IDashboardChartService
{
    Task<List<LeadTimePoint>> GenerateLeadTimeDataPointsAsync(
        Guid teamId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    Task<List<CycleTimePoint>> GenerateCycleTimeDataPointsAsync(
        Guid teamId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    Task<List<VelocityPoint>> GenerateVelocityDataPointsAsync(
        Guid teamId,
        CancellationToken cancellationToken = default);

    Task<(List<BurnupPoint> BurnupData, List<BurndownPoint> BurndownData)> GenerateBurnChartDataAsync(
        Guid teamId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);
}