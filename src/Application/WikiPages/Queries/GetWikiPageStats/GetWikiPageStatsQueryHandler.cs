using Application.Common.Interfaces.Persistence;
using ErrorOr;
using MediatR;

namespace Application.WikiPages.Queries.GetWikiPageStats;

internal sealed class GetWikiPageStatsQueryHandler(IWikiPagesRepository wikiPagesRepository)
    : IRequestHandler<GetWikiPageStatsQuery, ErrorOr<WikiPageStats>>
{
    public async Task<ErrorOr<WikiPageStats>> Handle(
        GetWikiPageStatsQuery query,
        CancellationToken cancellationToken)
    {
        var views = await wikiPagesRepository.GetViewsForLastDaysAsync(
            query.WikiPageId,
            query.PageViewsForDays,
            cancellationToken);

        return new WikiPageStats(views);
    }
}