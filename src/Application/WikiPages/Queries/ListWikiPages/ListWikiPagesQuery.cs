using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.WikiPages.Queries.ListWikiPages;

public sealed record ListWikiPagesQuery(Guid ProjectId)
    : IAuthorizationRequest<ErrorOr<IReadOnlyList<WikiPageBriefResponse>>>
{
    public Permission RequiredPermission => Permission.ProjectView;
    public ResourceType ResourceType => ResourceType.Project;
    public Guid ResourceId => ProjectId;
}
