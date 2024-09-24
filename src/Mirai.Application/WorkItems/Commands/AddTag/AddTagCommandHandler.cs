using ErrorOr;
using MediatR;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.Tags;
using Mirai.Domain.WorkItems;

namespace Mirai.Application.WorkItems.Commands.AddTag;

public class AddTagCommandHandler(
    IWorkItemsRepository _workItemsRepository,
    ITagsRepository _tagsRepository)
    : IRequestHandler<AddTagCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        AddTagCommand request,
        CancellationToken cancellationToken)
    {
        var workItem = await _workItemsRepository.GetByIdAsync(request.WorkItemId, cancellationToken);
        if (workItem is null)
        {
            return WorkItemErrors.WorkItemNotFound;
        }

        var tag = await _tagsRepository.GetByNameAsync(request.TagName, cancellationToken)
            ?? new Tag(request.TagName);

        workItem.AddTag(tag);
        await _workItemsRepository.UpdateAsync(workItem, cancellationToken);

        return Result.Success;
    }
}