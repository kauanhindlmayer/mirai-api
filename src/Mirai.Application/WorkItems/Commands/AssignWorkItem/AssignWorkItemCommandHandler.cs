using ErrorOr;
using MediatR;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.WorkItems;

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
            return WorkItemErrors.WorkItemNotFound;
        }

        var assignee = await _usersRepository.GetByIdAsync(request.AssigneeId, cancellationToken);
        if (assignee is null)
        {
            return WorkItemErrors.AssigneeNotFound;
        }

        workItem.Assign(request.AssigneeId);
        _workItemsRepository.Update(workItem);

        return Result.Success;
    }
}