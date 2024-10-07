using Application.Common.Interfaces;
using Domain.WorkItems;
using ErrorOr;
using MediatR;

namespace Application.WorkItems.Commands.AssignWorkItem;

public class AssignWorkItemCommandHandler(
    IWorkItemsRepository _workItemsRepository,
    IUsersRepository _usersRepository)
    : IRequestHandler<AssignWorkItemCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        AssignWorkItemCommand command,
        CancellationToken cancellationToken)
    {
        var workItem = await _workItemsRepository.GetByIdAsync(
            command.WorkItemId,
            cancellationToken);

        if (workItem is null)
        {
            return WorkItemErrors.NotFound;
        }

        var assignee = await _usersRepository.GetByIdAsync(
            command.AssigneeId,
            cancellationToken);

        if (assignee is null)
        {
            return WorkItemErrors.AssigneeNotFound;
        }

        workItem.Assign(command.AssigneeId);
        _workItemsRepository.Update(workItem);

        return Result.Success;
    }
}