using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.Retrospectives.Queries.GetRetrospective;

public sealed record GetRetrospectiveQuery(Guid RetrospectiveId)
    : IAuthorizationRequest<ErrorOr<RetrospectiveResponse>>
{
    public Permission RequiredPermission => Permission.TeamView;
    public ResourceType ResourceType => ResourceType.Retrospective;
    public Guid ResourceId => RetrospectiveId;
}
