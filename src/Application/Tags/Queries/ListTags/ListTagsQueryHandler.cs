using Application.Common;
using Application.Common.Interfaces.Persistence;
using Application.Common.Mappings;
using Application.Tags.Queries.Common;
using ErrorOr;
using MediatR;

namespace Application.Tags.Queries.ListTags;

internal sealed class ListTagsQueryHandler : IRequestHandler<ListTagsQuery, ErrorOr<PaginatedList<TagResponse>>>
{
    private readonly IApplicationDbContext _context;

    public ListTagsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<PaginatedList<TagResponse>>> Handle(
        ListTagsQuery query,
        CancellationToken cancellationToken)
    {
        var tagsQuery = _context.Tags.Where(t => t.ProjectId == query.ProjectId);
        query.SearchTerm ??= query.SearchTerm?.Trim().ToLower();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            tagsQuery = tagsQuery.Where(t => t.Name.ToLower().Contains(query.SearchTerm));
        }

        var tags = await tagsQuery
            .Select(TagQueries.ProjectToDto())
            .PaginatedListAsync(query.Page, query.PageSize, cancellationToken);

        return tags;
    }
}