using Domain.Tags;
using Domain.WorkItems;
using ErrorOr;
using MediatR;

namespace Application.WorkItems.Commands.AddTag;

internal sealed class AddTagCommandHandler
    : IRequestHandler<AddTagCommand, ErrorOr<Success>>
{
    private readonly IWorkItemRepository _workItemRepository;
    private readonly ITagRepository _tagsRepository;

    public AddTagCommandHandler(
        IWorkItemRepository workItemRepository,
        ITagRepository tagsRepository)
    {
        _workItemRepository = workItemRepository;
        _tagsRepository = tagsRepository;
    }

    public async Task<ErrorOr<Success>> Handle(
        AddTagCommand command,
        CancellationToken cancellationToken)
    {
        var workItem = await _workItemRepository.GetByIdWithTagsAsync(
            command.WorkItemId,
            cancellationToken);

        if (workItem is null)
        {
            return WorkItemErrors.NotFound;
        }

        var tag = await _tagsRepository.GetByNameAsync(command.TagName, cancellationToken)
            ?? new Tag(command.TagName, string.Empty, string.Empty, command.ProjectId);

        workItem.AddTag(tag);
        _workItemRepository.Update(workItem);

        return Result.Success;
    }
}