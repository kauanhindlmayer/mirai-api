using Microsoft.EntityFrameworkCore;

namespace Application.Common;

public sealed class PaginatedList<T>
{
    public int TotalCount { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public IReadOnlyList<T> Items { get; } = [];
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber * PageSize < TotalCount;
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    private PaginatedList(
        int totalCount,
        int pageNumber,
        int pageSize,
        IReadOnlyList<T> items)
    {
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
        Items = items;
    }

    public static async Task<PaginatedList<T>> CreateAsync(
        IQueryable<T> query,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<T>(totalCount, pageNumber, pageSize, items);
    }
}