using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.Boards.Commands.DeleteColumn;

public sealed record DeleteColumnCommand(
    Guid BoardId,
    Guid ColumnId) : IAuthorizationRequest<ErrorOr<Success>>
{
    public Permission RequiredPermission => Permission.TeamManageBoards;
    public ResourceType ResourceType => ResourceType.Board;
    public Guid ResourceId => BoardId;
}
