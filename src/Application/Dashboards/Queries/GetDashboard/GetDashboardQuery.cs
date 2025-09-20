using Application.Abstractions.Caching;
using ErrorOr;

namespace Application.Dashboards.Queries.GetDashboard;

public sealed record GetDashboardQuery(
    Guid TeamId,
    DateTime? StartDate,
    DateTime? EndDate) : ICachedQuery<ErrorOr<DashboardResponse>>
{
    public string CacheKey
    {
        get
        {
            var endDate = EndDate ?? DateTime.UtcNow;
            var startDate = StartDate ?? endDate.AddDays(-14);
            return CacheKeys.GetDashboardKey(TeamId, startDate, endDate);
        }
    }

    public TimeSpan? Expiration => TimeSpan.FromMinutes(10);
}
