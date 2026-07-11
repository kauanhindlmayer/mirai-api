using Application.Abstractions;
using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.WorkItems.Queries.GetWorkItemHistory;

public sealed record GetWorkItemHistoryQuery(
    Guid WorkItemId,
    int Page,
    int PageSize) : IAuthorizationRequest<ErrorOr<PaginatedList<WorkItemChangeSetResponse>>>
{
    public Permission RequiredPermission => Permission.ProjectView;
    public ResourceType ResourceType => ResourceType.WorkItem;
    public Guid ResourceId => WorkItemId;
}
