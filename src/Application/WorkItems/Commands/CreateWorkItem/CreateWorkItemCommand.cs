using Application.Abstractions.Authorization;
using Domain.Authorization;
using Domain.WorkItems.Enums;
using ErrorOr;

namespace Application.WorkItems.Commands.CreateWorkItem;

public sealed record CreateWorkItemCommand(
    Guid ProjectId,
    WorkItemType Type,
    string Title,
    Guid AssignedTeamId) : IAuthorizationRequest<ErrorOr<Guid>>
{
    public Permission RequiredPermission => Permission.ProjectManageWorkItems;
    public ResourceType ResourceType => ResourceType.Project;
    public Guid ResourceId => ProjectId;
}
