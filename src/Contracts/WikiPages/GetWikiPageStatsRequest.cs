namespace Contracts.WikiPages;

public sealed record GetWikiPageStatsRequest
{
    /// <summary>
    /// The number of days to get page views for.
    /// </summary>
    public int PageViewsForDays { get; init; }
}