using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Services;
using Application.WorkItems.Queries.Common;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;

namespace Application.WorkItems.Queries.SearchWorkItems;

internal sealed class SearchWorkItemsQueryHandler
    : IRequestHandler<SearchWorkItemsQuery, ErrorOr<IReadOnlyList<WorkItemBriefResponse>>>
{
    private readonly INlpService _nlpService;
    private readonly IApplicationDbContext _context;

    public SearchWorkItemsQueryHandler(
        INlpService nlpService,
        IApplicationDbContext context)
    {
        _nlpService = nlpService;
        _context = context;
    }

    public async Task<ErrorOr<IReadOnlyList<WorkItemBriefResponse>>> Handle(
        SearchWorkItemsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _nlpService.GenerateEmbeddingVectorAsync(query.SearchTerm);
        if (result.IsError)
        {
            return result.Errors;
        }

        var searchTermVector = new Vector(result.Value);

        var workItems = await _context.WorkItems
            .AsNoTracking()
            .Where(wi => wi.ProjectId == query.ProjectId && wi.SearchVector != null)
            .OrderBy(wi => wi.SearchVector!.CosineDistance(searchTermVector))
            .Select(WorkItemQueries.ProjectToBriefDto())
            .Take(10)
            .ToListAsync(cancellationToken);

        return workItems;
    }
}