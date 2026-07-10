using Application.Abstractions.Authorization;
using Domain.Authorization;
using Domain.Backlogs;
using ErrorOr;

namespace Application.Boards.Queries.GetColumnCards;

public sealed record GetColumnCardsQuery(
    Guid BoardId,
    Guid ColumnId,
    BacklogLevel? BacklogLevel = null,
    int PageSize = 20,
    int Page = 1) : IAuthorizationRequest<ErrorOr<ColumnCardsResponse>>
{
    public Permission RequiredPermission => Permission.ProjectView;
    public ResourceType ResourceType => ResourceType.Board;
    public Guid ResourceId => BoardId;
}
