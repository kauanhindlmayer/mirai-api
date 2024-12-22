using ErrorOr;
using MediatR;

namespace Application.WikiPages.Queries.GetWikiPageStats;

public sealed record GetWikiPageStatsQuery(
    Guid WikiPageId,
    int PageViewsForDays) : IRequest<ErrorOr<WikiPageStats>>;