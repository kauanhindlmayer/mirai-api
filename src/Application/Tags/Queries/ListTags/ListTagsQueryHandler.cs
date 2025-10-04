using Application.Abstractions;
using Application.Abstractions.Mappings;
using Application.Abstractions.Sorting;
using Domain.Shared;
using Domain.Tags;
using ErrorOr;
using MediatR;

namespace Application.Tags.Queries.ListTags;

internal sealed class ListTagsQueryHandler
    : IRequestHandler<ListTagsQuery, ErrorOr<PaginatedList<TagResponse>>>
{
    private readonly IApplicationDbContext _context;
    private readonly SortMappingProvider _sortMappingProvider;

    public ListTagsQueryHandler(
        IApplicationDbContext context,
        SortMappingProvider sortMappingProvider)
    {
        _context = context;
        _sortMappingProvider = sortMappingProvider;
    }

    public async Task<ErrorOr<PaginatedList<TagResponse>>> Handle(
        ListTagsQuery query,
        CancellationToken cancellationToken)
    {
        if (!_sortMappingProvider.ValidateMappings<TagResponse, Tag>(query.Sort))
        {
            return Errors.InvalidSort(query.Sort);
        }

        var tagsQuery = _context.Tags.Where(t => t.ProjectId == query.ProjectId);
        query.SearchTerm = query.SearchTerm?.Trim().ToLower();

        var sortMappings = _sortMappingProvider.GetMappings<TagResponse, Tag>();

        var tags = await tagsQuery
            .Where(t => query.SearchTerm == null || t.Name.ToLower().Contains(query.SearchTerm))
            .ApplySorting(query.Sort, sortMappings)
            .Select(TagQueries.ProjectToDto())
            .PaginatedListAsync(query.Page, query.PageSize, cancellationToken);

        return tags;
    }
}