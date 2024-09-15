using ErrorOr;
using MediatR;
using Mirai.Application.Common.Interfaces;

namespace Mirai.Application.WorkItems.Commands.AssignWorkItem;

public class AssignWorkItemCommandHandler(
    IWorkItemsRepository _workItemsRepository,
    IUsersRepository _usersRepository)
    : IRequestHandler<AssignWorkItemCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        AssignWorkItemCommand request,
        CancellationToken cancellationToken)
    {
        var workItem = await _workItemsRepository.GetByIdAsync(request.WorkItemId, cancellationToken);
        if (workItem is null)
        {
            return Error.NotFound(description: "Work item not found");
        }

        var assignee = await _usersRepository.GetByIdAsync(request.AssigneeId, cancellationToken);
        if (assignee is null)
        {
            return Error.NotFound(description: "Assignee not found");
        }

        workItem.Assign(request.AssigneeId);
        await _workItemsRepository.UpdateAsync(workItem, cancellationToken);

        return Result.Success;
    }
}