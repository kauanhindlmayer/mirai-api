using Application.Abstractions.Authorization;
using Domain.Authorization;
using Domain.Backlogs;
using ErrorOr;

namespace Application.Boards.Queries.GetBoard;

public sealed record GetBoardQuery(
    Guid BoardId,
    BacklogLevel? BacklogLevel = null,
    int PageSize = 20) : IAuthorizationRequest<ErrorOr<BoardResponse>>
{
    public Permission RequiredPermission => Permission.ProjectView;
    public ResourceType ResourceType => ResourceType.Board;
    public Guid ResourceId => BoardId;
}
