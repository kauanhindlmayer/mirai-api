using Application.Common.Interfaces;
using Domain.Tags;
using Domain.WorkItems;
using ErrorOr;
using MediatR;

namespace Application.WorkItems.Commands.AddTag;

public class AddTagCommandHandler(
    IWorkItemsRepository _workItemsRepository,
    ITagsRepository _tagsRepository)
    : IRequestHandler<AddTagCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        AddTagCommand command,
        CancellationToken cancellationToken)
    {
        var workItem = await _workItemsRepository.GetByIdAsync(
            command.WorkItemId,
            cancellationToken);

        if (workItem is null)
        {
            return WorkItemErrors.WorkItemNotFound;
        }

        var tag = await _tagsRepository.GetByNameAsync(command.TagName, cancellationToken)
            ?? new Tag(command.TagName);

        workItem.AddTag(tag);
        _workItemsRepository.Update(workItem);

        return Result.Success;
    }
}