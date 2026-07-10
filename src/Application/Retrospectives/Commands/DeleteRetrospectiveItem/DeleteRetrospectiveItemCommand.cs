using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.Retrospectives.Commands.DeleteRetrospectiveItem;

public sealed record DeleteRetrospectiveItemCommand(
    Guid RetrospectiveId,
    Guid ColumnId,
    Guid ItemId) : IAuthorizationRequest<ErrorOr<Success>>
{
    public Permission RequiredPermission => Permission.TeamView;
    public ResourceType ResourceType => ResourceType.Retrospective;
    public Guid ResourceId => RetrospectiveId;
}
