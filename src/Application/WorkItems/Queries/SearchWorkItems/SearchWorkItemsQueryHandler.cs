using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Services;
using Domain.WorkItems;
using ErrorOr;
using MediatR;

namespace Application.WorkItems.Queries.SearchWorkItems;

internal sealed class SearchWorkItemsQueryHandler : IRequestHandler<SearchWorkItemsQuery, ErrorOr<List<WorkItem>>>
{
    private readonly IEmbeddingService _embeddingService;
    private readonly IWorkItemsRepository _workItemsRepository;

    public SearchWorkItemsQueryHandler(
        IEmbeddingService embeddingService,
        IWorkItemsRepository workItemsRepository)
    {
        _embeddingService = embeddingService;
        _workItemsRepository = workItemsRepository;
    }

    public async Task<ErrorOr<List<WorkItem>>> Handle(
        SearchWorkItemsQuery query,
        CancellationToken cancellationToken)
    {
        var embeddingResult = await _embeddingService.GenerateEmbeddingAsync(query.SearchTerm);
        if (embeddingResult.IsError)
        {
            return embeddingResult.Errors;
        }

        var workItems = await _workItemsRepository.SearchAsync(
            query.ProjectId,
            embeddingResult.Value,
            cancellationToken: cancellationToken);

        return workItems;
    }
}