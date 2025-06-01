using Microsoft.EntityFrameworkCore;

namespace Application.Common;

public sealed class PaginatedList<T> : LinksResponse
{
    public IReadOnlyList<T> Items { get; }
    public int Page { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page * PageSize < TotalCount;

    private PaginatedList(
        int totalCount,
        int page,
        int pageSize,
        IReadOnlyList<T> items)
    {
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
        Items = items;
    }

    public static async Task<PaginatedList<T>> CreateAsync(
        IQueryable<T> query,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<T>(totalCount, page, pageSize, items);
    }
}