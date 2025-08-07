using Domain.Users;
using Domain.WorkItems;
using ErrorOr;
using MediatR;

namespace Application.WorkItems.Commands.AssignWorkItem;

internal sealed class AssignWorkItemCommandHandler
    : IRequestHandler<AssignWorkItemCommand, ErrorOr<Success>>
{
    private readonly IWorkItemsRepository _workItemsRepository;
    private readonly IUsersRepository _usersRepository;

    public AssignWorkItemCommandHandler(
        IWorkItemsRepository workItemsRepository,
        IUsersRepository usersRepository)
    {
        _workItemsRepository = workItemsRepository;
        _usersRepository = usersRepository;
    }

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