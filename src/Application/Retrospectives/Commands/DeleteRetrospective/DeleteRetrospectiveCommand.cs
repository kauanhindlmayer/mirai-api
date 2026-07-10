using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.Retrospectives.Commands.DeleteRetrospective;

public sealed record DeleteRetrospectiveCommand(Guid RetrospectiveId)
    : IAuthorizationRequest<ErrorOr<Success>>
{
    public Permission RequiredPermission => Permission.TeamManageRetrospectives;
    public ResourceType ResourceType => ResourceType.Retrospective;
    public Guid ResourceId => RetrospectiveId;
}
