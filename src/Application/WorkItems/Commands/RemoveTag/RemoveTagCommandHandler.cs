using Application.Common.Interfaces.Persistence;
using Domain.WorkItems;
using ErrorOr;
using MediatR;

namespace Application.WorkItems.Commands.RemoveTag;

public class RemoveTagCommandHandler(IWorkItemsRepository _workItemsRepository)
    : IRequestHandler<RemoveTagCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        RemoveTagCommand command,
        CancellationToken cancellationToken)
    {
        var workItem = await _workItemsRepository.GetByIdAsync(
            command.WorkItemId,
            cancellationToken);

        if (workItem is null)
        {
            return WorkItemErrors.NotFound;
        }

        var result = workItem.RemoveTag(command.TagName);
        if (result.IsError)
        {
            return result.Errors;
        }

        _workItemsRepository.Update(workItem);

        return Result.Success;
    }
}