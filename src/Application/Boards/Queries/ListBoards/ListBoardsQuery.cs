using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.Boards.Queries.ListBoards;

public sealed record ListBoardsQuery(Guid ProjectId)
    : IAuthorizationRequest<ErrorOr<IReadOnlyList<BoardBriefResponse>>>
{
    public Permission RequiredPermission => Permission.ProjectView;
    public ResourceType ResourceType => ResourceType.Project;
    public Guid ResourceId => ProjectId;
}
