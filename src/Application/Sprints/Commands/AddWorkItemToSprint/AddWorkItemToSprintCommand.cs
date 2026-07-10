using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.Sprints.Commands.AddWorkItemToSprint;

public sealed record AddWorkItemToSprintCommand(
    Guid TeamId,
    Guid SprintId,
    Guid WorkItemId) : IAuthorizationRequest<ErrorOr<Success>>
{
    public Permission RequiredPermission => Permission.TeamManageSprints;
    public ResourceType ResourceType => ResourceType.Team;
    public Guid ResourceId => TeamId;
}
