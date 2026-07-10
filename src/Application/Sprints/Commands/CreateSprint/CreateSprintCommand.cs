using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.Sprints.Commands.CreateSprint;

public sealed record CreateSprintCommand(
    Guid TeamId,
    string Name,
    DateOnly StartDate,
    DateOnly EndDate) : IAuthorizationRequest<ErrorOr<Guid>>
{
    public Permission RequiredPermission => Permission.TeamManageSprints;
    public ResourceType ResourceType => ResourceType.Team;
    public Guid ResourceId => TeamId;
}
