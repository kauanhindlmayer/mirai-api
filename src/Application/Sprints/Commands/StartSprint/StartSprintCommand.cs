using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.Sprints.Commands.StartSprint;

public sealed record StartSprintCommand(
    Guid TeamId,
    Guid SprintId) : IAuthorizationRequest<ErrorOr<Success>>
{
    public Permission RequiredPermission => Permission.TeamManageSprints;
    public ResourceType ResourceType => ResourceType.Team;
    public Guid ResourceId => TeamId;
}
