using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.WikiPages.Queries.GetWikiPageStats;

public sealed record GetWikiPageStatsQuery(
    Guid WikiPageId,
    int PageViewsForDays) : IAuthorizationRequest<ErrorOr<WikiPageStatsResponse>>
{
    public Permission RequiredPermission => Permission.ProjectView;
    public ResourceType ResourceType => ResourceType.WikiPage;
    public Guid ResourceId => WikiPageId;
}
