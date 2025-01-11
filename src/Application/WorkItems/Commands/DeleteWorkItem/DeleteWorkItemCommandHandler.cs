using Application.Common.Interfaces.Persistence;
using Domain.WorkItems;
using ErrorOr;
using MediatR;

namespace Application.WorkItems.Commands.DeleteWorkItem;

internal sealed class DeleteWorkItemCommandHandler : IRequestHandler<DeleteWorkItemCommand, ErrorOr<Success>>
{
    private readonly IWorkItemsRepository _workItemsRepository;

    public DeleteWorkItemCommandHandler(IWorkItemsRepository workItemsRepository)
    {
        _workItemsRepository = workItemsRepository;
    }

    public async Task<ErrorOr<Success>> Handle(
        DeleteWorkItemCommand command,
        CancellationToken cancellationToken)
    {
        var workItem = await _workItemsRepository.GetByIdAsync(
            command.WorkItemId,
            cancellationToken);

        if (workItem is null)
        {
            return WorkItemErrors.NotFound;
        }

        _workItemsRepository.Remove(workItem);

        return Result.Success;
    }
}