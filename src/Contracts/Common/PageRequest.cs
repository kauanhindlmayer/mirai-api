namespace Contracts.Common;

/// <summary>
/// Data transfer object for paginated requests.
/// </summary>
/// <param name="Page">The page number to retrieve.</param>
/// <param name="PageSize">The number of items per page.</param>
/// <param name="SortField">The field to sort the results by.</param>
/// <param name="SortOrder">The order to sort the results in (ascending or descending).</param>
/// <param name="SearchTerm">The term to search for in the results.</param>
public sealed record PageRequest(
    int Page = 1,
    int PageSize = 10,
    string? SortField = null,
    string? SortOrder = null,
    string? SearchTerm = null);