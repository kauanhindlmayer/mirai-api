using Application.Abstractions.Authorization;
using Domain.Authorization;
using Domain.WorkItems.Enums;
using Domain.WorkItems.ValueObjects;
using ErrorOr;

namespace Application.WorkItems.Commands.UpdateWorkItem;

public sealed record UpdateWorkItemCommand(
    Guid WorkItemId,
    WorkItemType? Type,
    string? Title,
    string? Description,
    string? AcceptanceCriteria,
    WorkItemStatus? Status,
    Guid? AssigneeId,
    Guid? AssignedTeamId,
    Guid? SprintId,
    Guid? ParentWorkItemId,
    Planning? Planning,
    Classification? Classification) : IAuthorizationRequest<ErrorOr<Guid>>
{
    public Permission RequiredPermission => Permission.ProjectManageWorkItems;
    public ResourceType ResourceType => ResourceType.WorkItem;
    public Guid ResourceId => WorkItemId;
}
