using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.WikiPages.Queries.GetWikiPage;

public sealed record GetWikiPageQuery(Guid WikiPageId) : IAuthorizationRequest<ErrorOr<WikiPageResponse>>
{
    public Permission RequiredPermission => Permission.ProjectView;
    public ResourceType ResourceType => ResourceType.WikiPage;
    public Guid ResourceId => WikiPageId;
}
