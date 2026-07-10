using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.Retrospectives.Queries.ListRetrospectives;

public sealed record ListRetrospectivesQuery(Guid TeamId)
    : IAuthorizationRequest<ErrorOr<IReadOnlyList<RetrospectiveBriefResponse>>>
{
    public Permission RequiredPermission => Permission.TeamView;
    public ResourceType ResourceType => ResourceType.Team;
    public Guid ResourceId => TeamId;
}
