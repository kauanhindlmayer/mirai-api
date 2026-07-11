using Domain.WorkItems;
using ErrorOr;
using MediatR;

namespace Application.WorkItems.Commands.UnassignWorkItem;

internal sealed class UnassignWorkItemCommandHandler
    : IRequestHandler<UnassignWorkItemCommand, ErrorOr<Success>>
{
    private readonly IWorkItemRepository _workItemRepository;

    public UnassignWorkItemCommandHandler(
        IWorkItemRepository workItemRepository)
    {
        _workItemRepository = workItemRepository;
    }

    public async Task<ErrorOr<Success>> Handle(
        UnassignWorkItemCommand command,
        CancellationToken cancellationToken)
    {
        var workItem = await _workItemRepository.GetByIdAsync(
            command.WorkItemId,
            cancellationToken);

        if (workItem is null)
        {
            return WorkItemErrors.NotFound;
        }

        workItem.UpdateAssignment(null);
        _workItemRepository.Update(workItem);

        return Result.Success;
    }
}
