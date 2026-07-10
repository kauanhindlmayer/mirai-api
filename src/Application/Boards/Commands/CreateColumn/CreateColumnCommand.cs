using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.Boards.Commands.CreateColumn;

public sealed record CreateColumnCommand(
    Guid BoardId,
    string Name,
    int? WipLimit,
    string DefinitionOfDone,
    int Position) : IAuthorizationRequest<ErrorOr<Guid>>
{
    public Permission RequiredPermission => Permission.TeamManageBoards;
    public ResourceType ResourceType => ResourceType.Board;
    public Guid ResourceId => BoardId;
}
