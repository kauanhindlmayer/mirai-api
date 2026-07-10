using Application.Abstractions.Authorization;
using Domain.Authorization;
using Domain.Retrospectives;
using ErrorOr;

namespace Application.Retrospectives.Commands.CreateRetrospectiveItem;

public sealed record CreateRetrospectiveItemCommand(
    string Content,
    Guid RetrospectiveId,
    Guid ColumnId) : IAuthorizationRequest<ErrorOr<RetrospectiveItem>>
{
    public Permission RequiredPermission => Permission.TeamView;
    public ResourceType ResourceType => ResourceType.Retrospective;
    public Guid ResourceId => RetrospectiveId;
}
