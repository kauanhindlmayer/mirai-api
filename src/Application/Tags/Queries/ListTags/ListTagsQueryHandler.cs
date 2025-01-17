using Application.Common.Interfaces.Persistence;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Tags.Queries.ListTags;

internal sealed class ListTagsQueryHandler : IRequestHandler<ListTagsQuery, ErrorOr<IReadOnlyList<TagResponse>>>
{
    private readonly IApplicationDbContext _context;

    public ListTagsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<IReadOnlyList<TagResponse>>> Handle(
        ListTagsQuery query,
        CancellationToken cancellationToken)
    {
        var tagsQuery = _context.Tags
            .AsNoTracking()
            .Where(t => t.ProjectId == query.ProjectId);

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            tagsQuery = tagsQuery.Where(t => t.Name.Contains(query.SearchTerm));
        }

        var tags = await tagsQuery
            .Select(t => new TagResponse
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                Color = t.Color,
                WorkItemsCount = t.WorkItems.Count,
            })
            .ToListAsync(cancellationToken);

        return tags;
    }
}