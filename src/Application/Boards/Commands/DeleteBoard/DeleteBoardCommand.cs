using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.Boards.Commands.DeleteBoard;

public sealed record DeleteBoardCommand(Guid BoardId) : IAuthorizationRequest<ErrorOr<Success>>
{
    public Permission RequiredPermission => Permission.TeamManageBoards;
    public ResourceType ResourceType => ResourceType.Board;
    public Guid ResourceId => BoardId;
}
