using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Services;
using Domain.WorkItems;
using ErrorOr;
using MediatR;

namespace Application.WorkItems.Queries.SearchWorkItems;

internal sealed class SearchWorkItemsQueryHandler(
    IEmbeddingService embeddingService,
    IWorkItemsRepository workItemsRepository)
    : IRequestHandler<SearchWorkItemsQuery, ErrorOr<List<WorkItemSummary>>>
{
    public async Task<ErrorOr<List<WorkItemSummary>>> Handle(
        SearchWorkItemsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await embeddingService.GenerateEmbeddingAsync(query.SearchTerm);
        if (result.IsError)
        {
            return result.Errors;
        }

        var workItems = await workItemsRepository.SearchAsync(
            query.ProjectId,
            result.Value,
            cancellationToken: cancellationToken);

        return workItems;
    }
}