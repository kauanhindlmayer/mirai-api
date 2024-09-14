using ErrorOr;
using MediatR;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.WorkItems;

namespace Mirai.Application.WorkItems.Queries.GetWorkItem;

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
            return Error.NotFound(description: "Work Item not found");
        }

        return workItem;
    }
}