using Application.Common;
using Application.Common.Interfaces.Persistence;
using Domain.WorkItems;
using ErrorOr;
using MediatR;

namespace Application.WorkItems.Queries.ListWorkItems;

internal sealed class ListWorkItemsQueryHandler(IWorkItemsRepository workItemsRepository)
    : IRequestHandler<ListWorkItemsQuery, ErrorOr<PagedList<WorkItem>>>
{
    public async Task<ErrorOr<PagedList<WorkItem>>> Handle(
        ListWorkItemsQuery query,
        CancellationToken cancellationToken)
    {
        var workItems = await workItemsRepository.PaginatedListAsync(
            query.ProjectId,
            query.PageNumber,
            query.PageSize,
            query.SortField,
            query.SortOrder,
            query.SearchTerm,
            cancellationToken);

        return workItems;
    }
}