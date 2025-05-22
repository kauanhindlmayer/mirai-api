namespace WebApi.Controllers.WikiPages;

/// <summary>
/// Data transfer object for getting wiki page statistics.
/// </summary>
/// <param name="PageViewsForDays">The number of days to get page views for.</param>
public sealed record GetWikiPageStatsRequest(int PageViewsForDays);