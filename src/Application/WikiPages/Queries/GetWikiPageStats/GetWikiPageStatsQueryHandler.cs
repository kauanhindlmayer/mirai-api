using Application.Common.Interfaces.Persistence;
using ErrorOr;
using MediatR;

namespace Application.WikiPages.Queries.GetWikiPageStats;

internal sealed class GetWikiPageStatsQueryHandler
    : IRequestHandler<GetWikiPageStatsQuery, ErrorOr<WikiPageStatsResponse>>
{
    private readonly IWikiPagesRepository _wikiPagesRepository;

    public GetWikiPageStatsQueryHandler(IWikiPagesRepository wikiPagesRepository)
    {
        _wikiPagesRepository = wikiPagesRepository;
    }

    public async Task<ErrorOr<WikiPageStatsResponse>> Handle(
        GetWikiPageStatsQuery query,
        CancellationToken cancellationToken)
    {
        var views = await _wikiPagesRepository.GetViewsForLastDaysAsync(
            query.WikiPageId,
            query.PageViewsForDays,
            cancellationToken);

        return new WikiPageStatsResponse(views);
    }
}