using Application.Abstractions.Authorization;
using Domain.Authorization;
using Domain.Retrospectives.Enums;
using ErrorOr;

namespace Application.Retrospectives.Commands.CreateRetrospective;

public sealed record CreateRetrospectiveCommand(
    string Title,
    int? MaxVotesPerUser,
    RetrospectiveTemplate? Template,
    Guid TeamId) : IAuthorizationRequest<ErrorOr<Guid>>
{
    public Permission RequiredPermission => Permission.TeamManageRetrospectives;
    public ResourceType ResourceType => ResourceType.Team;
    public Guid ResourceId => TeamId;
}
