using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.Boards.Commands.MoveCard;

public sealed record MoveCardCommand(
    Guid BoardId,
    Guid ColumnId,
    Guid CardId,
    Guid TargetColumnId,
    int TargetPosition) : IAuthorizationRequest<ErrorOr<Success>>
{
    public Permission RequiredPermission => Permission.ProjectManageWorkItems;
    public ResourceType ResourceType => ResourceType.Board;
    public Guid ResourceId => BoardId;
}
