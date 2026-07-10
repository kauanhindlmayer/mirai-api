using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.WorkItems.Queries.GetWorkItem;

public sealed record GetWorkItemQuery(Guid WorkItemId) : IAuthorizationRequest<ErrorOr<WorkItemResponse>>
{
    public Permission RequiredPermission => Permission.ProjectView;
    public ResourceType ResourceType => ResourceType.WorkItem;
    public Guid ResourceId => WorkItemId;
}
