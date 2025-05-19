using System.Linq.Dynamic.Core;

namespace Application.Common.Sorting;

internal static class QueryableExtensions
{
    public static IQueryable<T> ApplySorting<T>(
        this IQueryable<T> query,
        string? sort,
        SortMapping[] mappings,
        string defaultOrderBy = "Id")
    {
        if (string.IsNullOrWhiteSpace(sort))
        {
            return query.OrderBy(defaultOrderBy);
        }

        var sortFields = sort.Split(',')
            .Select(s => s.Trim())
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToArray();

        var orderByParts = new List<string>();
        foreach (string field in sortFields)
        {
            (string sortField, bool isDescending) = ParseSortField(field);

            var mapping = mappings.First(m =>
                m.SortField.Equals(sortField, StringComparison.OrdinalIgnoreCase));

            var direction = (isDescending, mapping.Reverse) switch
            {
                (false, false) => "ASC",
                (false, true) => "DESC",
                (true, false) => "DESC",
                (true, true) => "ASC",
            };

            orderByParts.Add($"{mapping.PropertyName} {direction}");
        }

        var orderBy = string.Join(",", orderByParts);

        return query.OrderBy(orderBy);
    }

    private static (string SortField, bool IsDescending) ParseSortField(string field)
    {
        var parts = field.Split(' ');
        var sortField = parts[0];
        var isDescending = parts.Length > 1 &&
                           parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase);

        return (sortField, isDescending);
    }
}
