using ErrorOr;
using MediatR;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.WorkItems;

namespace Mirai.Application.WorkItems.Commands.RemoveTag;

public class RemoveTagCommandHandler(IWorkItemsRepository _workItemsRepository)
    : IRequestHandler<RemoveTagCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        RemoveTagCommand request,
        CancellationToken cancellationToken)
    {
        var workItem = await _workItemsRepository.GetByIdAsync(
            request.WorkItemId,
            cancellationToken);

        if (workItem is null)
        {
            return WorkItemErrors.WorkItemNotFound;
        }

        var result = workItem.RemoveTag(request.TagName);
        if (result.IsError)
        {
            return result.Errors;
        }

        _workItemsRepository.Update(workItem);

        return Result.Success;
    }
}