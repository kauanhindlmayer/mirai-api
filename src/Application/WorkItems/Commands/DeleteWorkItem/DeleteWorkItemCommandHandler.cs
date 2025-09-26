using Domain.WorkItems;
using ErrorOr;
using MediatR;

namespace Application.WorkItems.Commands.DeleteWorkItem;

internal sealed class DeleteWorkItemCommandHandler
    : IRequestHandler<DeleteWorkItemCommand, ErrorOr<Success>>
{
    private readonly IWorkItemRepository _workItemRepository;

    public DeleteWorkItemCommandHandler(
        IWorkItemRepository workItemRepository)
    {
        _workItemRepository = workItemRepository;
    }

    public async Task<ErrorOr<Success>> Handle(
        DeleteWorkItemCommand command,
        CancellationToken cancellationToken)
    {
        var workItem = await _workItemRepository.GetByIdAsync(
            command.WorkItemId,
            cancellationToken);

        if (workItem is null)
        {
            return WorkItemErrors.NotFound;
        }

        _workItemRepository.Remove(workItem);

        return Result.Success;
    }
}