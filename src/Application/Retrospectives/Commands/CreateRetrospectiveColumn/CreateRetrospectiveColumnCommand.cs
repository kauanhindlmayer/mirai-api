using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.Retrospectives.Commands.CreateRetrospectiveColumn;

public sealed record CreateRetrospectiveColumnCommand(
    string Title,
    Guid RetrospectiveId) : IAuthorizationRequest<ErrorOr<Guid>>
{
    public Permission RequiredPermission => Permission.TeamManageRetrospectives;
    public ResourceType ResourceType => ResourceType.Retrospective;
    public Guid ResourceId => RetrospectiveId;
}
