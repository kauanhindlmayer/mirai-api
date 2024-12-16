using Application.Common.Interfaces.Persistence;
using Domain.WorkItems;
using ErrorOr;
using MediatR;

namespace Application.WorkItems.Commands.DeleteWorkItem;

internal sealed class DeleteWorkItemCommandHandler(IWorkItemsRepository workItemsRepository)
    : IRequestHandler<DeleteWorkItemCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        DeleteWorkItemCommand command,
        CancellationToken cancellationToken)
    {
        var workItem = await workItemsRepository.GetByIdAsync(
            command.WorkItemId,
            cancellationToken);

        if (workItem is null)
        {
            return WorkItemErrors.NotFound;
        }

        workItemsRepository.Remove(workItem);

        return Result.Success;
    }
}