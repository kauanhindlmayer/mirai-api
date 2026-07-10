using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.WorkItems.Commands.AddTag;

public sealed record AddTagCommand(
    Guid ProjectId,
    Guid WorkItemId,
    string TagName) : IAuthorizationRequest<ErrorOr<Success>>
{
    public Permission RequiredPermission => Permission.ProjectManageWorkItems;
    public ResourceType ResourceType => ResourceType.WorkItem;
    public Guid ResourceId => WorkItemId;
}
