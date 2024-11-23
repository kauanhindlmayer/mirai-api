using Application.Common.Interfaces.Persistence;
using Domain.WorkItems;
using ErrorOr;
using MediatR;

namespace Application.WorkItems.Commands.AssignWorkItem;

internal sealed class AssignWorkItemCommandHandler(
    IWorkItemsRepository workItemsRepository,
    IUsersRepository usersRepository)
    : IRequestHandler<AssignWorkItemCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        AssignWorkItemCommand command,
        CancellationToken cancellationToken)
    {
        var workItem = await workItemsRepository.GetByIdAsync(
            command.WorkItemId,
            cancellationToken);

        if (workItem is null)
        {
            return WorkItemErrors.NotFound;
        }

        var assignee = await usersRepository.GetByIdAsync(
            command.AssigneeId,
            cancellationToken);

        if (assignee is null)
        {
            return WorkItemErrors.AssigneeNotFound;
        }

        workItem.Assign(command.AssigneeId);
        workItemsRepository.Update(workItem);

        return Result.Success;
    }
}