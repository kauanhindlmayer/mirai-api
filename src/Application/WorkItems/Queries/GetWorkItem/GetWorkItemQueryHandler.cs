using Application.Common.Interfaces.Persistence;
using Domain.WorkItems;
using ErrorOr;
using MediatR;

namespace Application.WorkItems.Queries.GetWorkItem;

internal sealed class GetWorkItemQueryHandler(IWorkItemsRepository workItemsRepository)
    : IRequestHandler<GetWorkItemQuery, ErrorOr<WorkItem>>
{
    public async Task<ErrorOr<WorkItem>> Handle(
        GetWorkItemQuery query,
        CancellationToken cancellationToken)
    {
        var workItem = await workItemsRepository.GetByIdAsync(
            query.WorkItemId,
            cancellationToken);

        if (workItem is null)
        {
            return WorkItemErrors.NotFound;
        }

        return workItem;
    }
}