using Domain.WikiPages;
using ErrorOr;
using MediatR;

namespace Application.WikiPages.Queries.GetWikiPageStats;

internal sealed class GetWikiPageStatsQueryHandler
    : IRequestHandler<GetWikiPageStatsQuery, ErrorOr<WikiPageStatsResponse>>
{
    private readonly IWikiPageRepository _wikiPageRepository;

    public GetWikiPageStatsQueryHandler(IWikiPageRepository wikiPageRepository)
    {
        _wikiPageRepository = wikiPageRepository;
    }

    public async Task<ErrorOr<WikiPageStatsResponse>> Handle(
        GetWikiPageStatsQuery query,
        CancellationToken cancellationToken)
    {
        var views = await _wikiPageRepository.GetViewsForLastDaysAsync(
            query.WikiPageId,
            query.PageViewsForDays,
            cancellationToken);

        return new WikiPageStatsResponse(views);
    }
}