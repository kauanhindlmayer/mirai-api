using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.WorkItems.Commands.UnassignWorkItem;

public sealed record UnassignWorkItemCommand(
    Guid WorkItemId) : IAuthorizationRequest<ErrorOr<Success>>
{
    public Permission RequiredPermission => Permission.ProjectManageWorkItems;
    public ResourceType ResourceType => ResourceType.WorkItem;
    public Guid ResourceId => WorkItemId;
}
