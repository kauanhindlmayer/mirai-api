using Application.Abstractions.Authorization;
using Application.Abstractions.Caching;
using Domain.Authorization;
using ErrorOr;

namespace Application.Dashboards.Queries.GetDashboard;

public sealed record GetDashboardQuery(
    Guid TeamId,
    DateTime? StartDate,
    DateTime? EndDate) : ICachedQuery<ErrorOr<DashboardResponse>>, IAuthorizationRequest<ErrorOr<DashboardResponse>>
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

    public Permission RequiredPermission => Permission.TeamView;

    public ResourceType ResourceType => ResourceType.Team;

    public Guid ResourceId => TeamId;
}
