using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.Teams.Queries.ListTeams;

public sealed record ListTeamsQuery(Guid ProjectId)
    : IAuthorizationRequest<ErrorOr<IReadOnlyList<TeamBriefResponse>>>
{
    public Permission RequiredPermission => Permission.ProjectView;
    public ResourceType ResourceType => ResourceType.Project;
    public Guid ResourceId => ProjectId;
}
