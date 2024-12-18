using Microsoft.EntityFrameworkCore;

namespace Application.Common;

public sealed class PagedList<T>
{
    public int TotalCount { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public IReadOnlyList<T> Items { get; } = [];
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber * PageSize < TotalCount;

    private PagedList(
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

    public static async Task<PagedList<T>> CreateAsync(
        IQueryable<T> query,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedList<T>(totalCount, pageNumber, pageSize, items);
    }

    public PagedList<TOutput> Map<TOutput>(Func<T, TOutput> map)
    {
        return new PagedList<TOutput>(
            TotalCount,
            PageNumber,
            PageSize,
            Items.Select(map).ToList());
    }
}