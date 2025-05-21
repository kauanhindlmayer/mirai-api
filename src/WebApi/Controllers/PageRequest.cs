using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

/// <summary>
/// Data transfer object for paginated requests.
/// </summary>
public class PageRequest
{
    /// <summary>
    /// The page number to retrieve.
    /// </summary>
    public int Page { get; init; } = 1;

    /// <summary>
    /// The number of items per page.
    /// </summary>
    public int PageSize { get; init; } = 10;

    /// <summary>
    /// Sorting criteria in the format "field direction", e.g., "name asc, date desc".
    /// </summary>
    public string? Sort { get; init; }

    /// <summary>
    /// The term to search for in the results.
    /// </summary>
    [FromQuery(Name = "q")]
    public string? SearchTerm { get; init; }
}
