using Application.Common.Interfaces.Persistence;
using Domain.Tags;
using Domain.WorkItems;
using ErrorOr;
using MediatR;

namespace Application.WorkItems.Commands.AddTag;

internal sealed class AddTagCommandHandler : IRequestHandler<AddTagCommand, ErrorOr<Success>>
{
    private readonly IWorkItemsRepository _workItemsRepository;
    private readonly ITagsRepository _tagsRepository;

    public AddTagCommandHandler(
        IWorkItemsRepository workItemsRepository,
        ITagsRepository tagsRepository)
    {
        _workItemsRepository = workItemsRepository;
        _tagsRepository = tagsRepository;
    }

    public async Task<ErrorOr<Success>> Handle(
        AddTagCommand command,
        CancellationToken cancellationToken)
    {
        var workItem = await _workItemsRepository.GetByIdWithTagsAsync(
            command.WorkItemId,
            cancellationToken);

        if (workItem is null)
        {
            return WorkItemErrors.NotFound;
        }

        var tag = await _tagsRepository.GetByNameAsync(command.TagName, cancellationToken)
            ?? new Tag(command.TagName, string.Empty, string.Empty);

        workItem.AddTag(tag);
        _workItemsRepository.Update(workItem);

        return Result.Success;
    }
}