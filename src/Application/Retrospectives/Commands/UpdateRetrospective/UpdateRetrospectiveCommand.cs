using Application.Abstractions.Authorization;
using Domain.Authorization;
using Domain.Retrospectives.Enums;
using ErrorOr;

namespace Application.Retrospectives.Commands.UpdateRetrospective;

public sealed record UpdateRetrospectiveCommand(
    Guid RetrospectiveId,
    string Title,
    int? MaxVotesPerUser,
    RetrospectiveTemplate? Template) : IAuthorizationRequest<ErrorOr<Guid>>
{
    public Permission RequiredPermission => Permission.TeamManageRetrospectives;
    public ResourceType ResourceType => ResourceType.Retrospective;
    public Guid ResourceId => RetrospectiveId;
}
