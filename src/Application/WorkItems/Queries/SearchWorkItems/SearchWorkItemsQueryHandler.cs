using Application.Abstractions;
using Domain.Shared;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Pgvector;

namespace Application.WorkItems.Queries.SearchWorkItems;

internal sealed class SearchWorkItemsQueryHandler
    : IRequestHandler<SearchWorkItemsQuery, ErrorOr<IReadOnlyList<WorkItemResponseWithDistance>>>
{
    private const double MaxDistance = 0.6;
    private const int MaxItems = 10;
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;
    private readonly IApplicationDbContext _context;

    public SearchWorkItemsQueryHandler(
        [FromKeyedServices(ServiceKeys.Embedding)] IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
        IApplicationDbContext context)
    {
        _embeddingGenerator = embeddingGenerator;
        _context = context;
    }

    public async Task<ErrorOr<IReadOnlyList<WorkItemResponseWithDistance>>> Handle(
        SearchWorkItemsQuery query,
        CancellationToken cancellationToken)
    {
        var embedding = await _embeddingGenerator.GenerateAsync(
            query.SearchTerm,
            cancellationToken: cancellationToken);

        var searchTermVector = new Vector(embedding.Vector.ToArray());

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