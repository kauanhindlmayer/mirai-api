using Microsoft.EntityFrameworkCore;

namespace Application.Common.Mappings;

public static class MappingExtensions
{
    public static Task<PaginatedList<TDestination>> PaginatedListAsync<TDestination>(
        this IQueryable<TDestination> queryable,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
        where TDestination : class
    {
        return PaginatedList<TDestination>.CreateAsync(
            queryable.AsNoTracking(),
            page,
            pageSize,
            cancellationToken);
    }
}