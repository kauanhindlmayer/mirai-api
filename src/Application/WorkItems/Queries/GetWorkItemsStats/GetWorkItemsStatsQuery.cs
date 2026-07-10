using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.WorkItems.Queries.GetWorkItemsStats;

public sealed record GetWorkItemsStatsQuery(
    Guid ProjectId,
    int PeriodInDays) : IAuthorizationRequest<ErrorOr<WorkItemsStatsResponse>>
{
    public Permission RequiredPermission => Permission.ProjectView;
    public ResourceType ResourceType => ResourceType.Project;
    public Guid ResourceId => ProjectId;
}
