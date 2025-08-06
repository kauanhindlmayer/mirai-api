namespace Presentation.Controllers.WikiPages;

/// <summary>
/// Request to get statistics for a wiki page.
/// </summary>
/// <param name="PageViewsForDays">The number of days to get page views for.</param>
public sealed record GetWikiPageStatsRequest(int PageViewsForDays);