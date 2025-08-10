using Domain.Users;
using Domain.WorkItems;
using ErrorOr;
using MediatR;

namespace Application.WorkItems.Commands.AssignWorkItem;

internal sealed class AssignWorkItemCommandHandler
    : IRequestHandler<AssignWorkItemCommand, ErrorOr<Success>>
{
    private readonly IWorkItemRepository _workItemRepository;
    private readonly IUserRepository _userRepository;

    public AssignWorkItemCommandHandler(
        IWorkItemRepository workItemRepository,
        IUserRepository userRepository)
    {
        _workItemRepository = workItemRepository;
        _userRepository = userRepository;
    }

    public async Task<ErrorOr<Success>> Handle(
        AssignWorkItemCommand command,
        CancellationToken cancellationToken)
    {
        var workItem = await _workItemRepository.GetByIdAsync(
            command.WorkItemId,
            cancellationToken);

        if (workItem is null)
        {
            return WorkItemErrors.NotFound;
        }

        var assignee = await _userRepository.GetByIdAsync(
            command.AssigneeId,
            cancellationToken);

        if (assignee is null)
        {
            return WorkItemErrors.AssigneeNotFound;
        }

        workItem.Assign(command.AssigneeId);
        _workItemRepository.Update(workItem);

        return Result.Success;
    }
}