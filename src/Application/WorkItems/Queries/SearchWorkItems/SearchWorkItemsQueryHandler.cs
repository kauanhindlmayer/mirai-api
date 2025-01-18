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
    private readonly IEmbeddingService _embeddingService;
    private readonly IApplicationDbContext _context;

    public SearchWorkItemsQueryHandler(
        IEmbeddingService embeddingService,
        IApplicationDbContext context)
    {
        _embeddingService = embeddingService;
        _context = context;
    }

    public async Task<ErrorOr<IReadOnlyList<WorkItemBriefResponse>>> Handle(
        SearchWorkItemsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _embeddingService.GenerateEmbeddingAsync(query.SearchTerm);
        if (result.IsError)
        {
            return result.Errors;
        }

        var searchTermVector = new Vector(result.Value);

        var workItems = await _context.WorkItems
            .AsNoTracking()
            .Where(wi => wi.ProjectId == query.ProjectId && wi.SearchVector != null)
            .OrderBy(wi => wi.SearchVector!.CosineDistance(searchTermVector))
            .Select(wi => new WorkItemBriefResponse
            {
                Id = wi.Id,
                Code = wi.Code,
                Title = wi.Title,
                Status = wi.Status.Name,
                Type = wi.Type.Name,
                CreatedAt = wi.CreatedAt,
                UpdatedAt = wi.UpdatedAt,
            })
            .Take(10)
            .ToListAsync(cancellationToken);

        return workItems;
    }
}