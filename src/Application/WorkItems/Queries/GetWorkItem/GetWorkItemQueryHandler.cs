using Application.Common.Interfaces.Persistence;
using Domain.WorkItems;
using ErrorOr;
using MediatR;

namespace Application.WorkItems.Queries.GetWorkItem;

public class GetWorkItemQueryHandler(IWorkItemsRepository _workItemsRepository)
    : IRequestHandler<GetWorkItemQuery, ErrorOr<WorkItem>>
{
    public async Task<ErrorOr<WorkItem>> Handle(
        GetWorkItemQuery query,
        CancellationToken cancellationToken)
    {
        var workItem = await _workItemsRepository.GetByIdAsync(
            query.WorkItemId,
            cancellationToken);

        if (workItem is null)
        {
            return WorkItemErrors.NotFound;
        }

        return workItem;
    }
}