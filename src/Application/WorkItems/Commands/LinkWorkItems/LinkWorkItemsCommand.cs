using Application.Abstractions.Authorization;
using Domain.Authorization;
using Domain.WorkItems.Enums;
using ErrorOr;

namespace Application.WorkItems.Commands.LinkWorkItems;

public sealed record LinkWorkItemsCommand(
    Guid SourceWorkItemId,
    Guid TargetWorkItemId,
    WorkItemLinkType LinkType,
    string? Comment) : IAuthorizationRequest<ErrorOr<Guid>>
{
    public Permission RequiredPermission => Permission.ProjectManageWorkItems;
    public ResourceType ResourceType => ResourceType.WorkItem;
    public Guid ResourceId => SourceWorkItemId;
}
