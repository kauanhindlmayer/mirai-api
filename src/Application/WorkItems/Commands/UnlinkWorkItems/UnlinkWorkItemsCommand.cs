using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.WorkItems.Commands.UnlinkWorkItems;

public sealed record UnlinkWorkItemsCommand(
    Guid WorkItemId,
    Guid LinkId) : IAuthorizationRequest<ErrorOr<Success>>
{
    public Permission RequiredPermission => Permission.ProjectManageWorkItems;
    public ResourceType ResourceType => ResourceType.WorkItem;
    public Guid ResourceId => WorkItemId;
}
