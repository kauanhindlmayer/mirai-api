using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Services;
using Application.WorkItems.Queries.Common;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pgvector;

namespace Application.WorkItems.Queries.SearchWorkItems;

internal sealed class SearchWorkItemsQueryHandler
    : IRequestHandler<SearchWorkItemsQuery, ErrorOr<IReadOnlyList<WorkItemResponseWithDistance>>>
{
    private const double MaxDistance = 0.6;
    private const int MaxItems = 4;
    private readonly INlpService _nlpService;
    private readonly IApplicationDbContext _context;

    public SearchWorkItemsQueryHandler(
        INlpService nlpService,
        IApplicationDbContext context)
    {
        _nlpService = nlpService;
        _context = context;
    }

    public async Task<ErrorOr<IReadOnlyList<WorkItemResponseWithDistance>>> Handle(
        SearchWorkItemsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _nlpService.GenerateEmbeddingVectorAsync(
            query.SearchTerm,
            cancellationToken);

        if (result.IsError)
        {
            return result.Errors;
        }

        var searchTermVector = new Vector(result.Value);

        var workItems = await _context.WorkItems
            .AsNoTracking()
            .Where(wi => wi.ProjectId == query.ProjectId)
            .Select(WorkItemQueries.ProjectToDtoWithDistance(searchTermVector))
            .Where(x => x.Distance <= MaxDistance)
            .OrderBy(x => x.Distance)
            .Take(MaxItems)
            .ToListAsync(cancellationToken);

        return workItems;
    }
}