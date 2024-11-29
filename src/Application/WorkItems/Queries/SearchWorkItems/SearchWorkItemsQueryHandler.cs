using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Services;
using Domain.WorkItems;
using ErrorOr;
using MediatR;

namespace Application.WorkItems.Queries.SearchWorkItems;

internal sealed class SearchWorkItemsQueryHandler(
    IEmbeddingService embeddingService,
    IWorkItemsRepository workItemsRepository)
    : IRequestHandler<SearchWorkItemsQuery, ErrorOr<List<WorkItem>>>
{
    public async Task<ErrorOr<List<WorkItem>>> Handle(
        SearchWorkItemsQuery query,
        CancellationToken cancellationToken)
    {
        var embeddingResult = await embeddingService.GenerateEmbeddingAsync(query.SearchTerm);
        if (embeddingResult.IsError)
        {
            return embeddingResult.Errors;
        }

        var workItems = await workItemsRepository.SearchAsync(
            query.ProjectId,
            embeddingResult.Value,
            cancellationToken: cancellationToken);

        return workItems;
    }
}