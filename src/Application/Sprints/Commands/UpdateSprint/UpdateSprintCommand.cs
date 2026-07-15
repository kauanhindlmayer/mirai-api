using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.Sprints.Commands.UpdateSprint;

public sealed record UpdateSprintCommand(
    Guid TeamId,
    Guid SprintId,
    string Name,
    DateOnly StartDate,
    DateOnly EndDate) : IAuthorizationRequest<ErrorOr<Success>>
{
    public Permission RequiredPermission => Permission.TeamManageSprints;
    public ResourceType ResourceType => ResourceType.Team;
    public Guid ResourceId => TeamId;
}
