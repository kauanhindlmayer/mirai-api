using Application.Common.Interfaces.Persistence;
using Domain.Tags;
using Domain.WorkItems;
using ErrorOr;
using MediatR;

namespace Application.WorkItems.Commands.AddTag;

internal sealed class AddTagCommandHandler(
    IWorkItemsRepository workItemsRepository,
    ITagsRepository tagsRepository)
    : IRequestHandler<AddTagCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        AddTagCommand command,
        CancellationToken cancellationToken)
    {
        var workItem = await workItemsRepository.GetByIdAsync(
            command.WorkItemId,
            cancellationToken);

        if (workItem is null)
        {
            return WorkItemErrors.NotFound;
        }

        var tag = await tagsRepository.GetByNameAsync(command.TagName, cancellationToken)
            ?? new Tag(command.TagName);

        workItem.AddTag(tag);
        workItemsRepository.Update(workItem);

        return Result.Success;
    }
}