namespace Contracts.Common;

/// <summary>
/// Data transfer object for paginated requests.
/// </summary>
/// <param name="Page">The page number to retrieve.</param>
/// <param name="PageSize">The number of items per page.</param>
/// <param name="Sort">Sorting criteria in the format "field direction", e.g., "name asc, date desc".</param>
/// <param name="SearchTerm">The term to search for in the results.</param>
public record PageRequest(
    int Page = 1,
    int PageSize = 10,
    string? Sort = null,
    string? SearchTerm = null);