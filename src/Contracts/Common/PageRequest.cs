namespace Contracts.Common;

public sealed record PageRequest(
    int PageNumber = 1,
    int PageSize = 10,
    string? SortField = null,
    string? SortOrder = null,
    string? SearchTerm = null);