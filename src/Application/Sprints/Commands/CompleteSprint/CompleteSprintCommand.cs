using Application.Abstractions.Authorization;
using Domain.Authorization;
using Domain.Sprints;
using ErrorOr;

namespace Application.Sprints.Commands.CompleteSprint;

public sealed record CompleteSprintCommand(
    Guid TeamId,
    Guid SprintId,
    IReadOnlyList<SprintItemDisposition> Dispositions) : IAuthorizationRequest<ErrorOr<Success>>
{
    public Permission RequiredPermission => Permission.TeamManageSprints;
    public ResourceType ResourceType => ResourceType.Team;
    public Guid ResourceId => TeamId;
}
