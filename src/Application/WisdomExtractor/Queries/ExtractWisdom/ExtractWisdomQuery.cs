using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.WisdomExtractor.Queries.ExtractWisdom;

public sealed record ExtractWisdomQuery(
    Guid ProjectId,
    string Question) : IAuthorizationRequest<ErrorOr<WisdomResponse>>
{
    public Permission RequiredPermission => Permission.ProjectView;
    public ResourceType ResourceType => ResourceType.Project;
    public Guid ResourceId => ProjectId;
}
