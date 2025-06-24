using Application.Common.Interfaces.Persistence;
using Domain.Tags;
using ErrorOr;
using MediatR;

namespace Application.Tags.Commands.MergeTags;

internal sealed class MergeTagsCommandHandler : IRequestHandler<MergeTagsCommand, ErrorOr<Success>>
{
    private readonly ITagsRepository _tagRepository;
    private readonly IWorkItemsRepository _workItemRepository;

    public MergeTagsCommandHandler(
        ITagsRepository tagRepository,
        IWorkItemsRepository workItemRepository)
    {
        _tagRepository = tagRepository;
        _workItemRepository = workItemRepository;
    }

    public async Task<ErrorOr<Success>> Handle(
        MergeTagsCommand command,
        CancellationToken cancellationToken)
    {
        if (command.SourceTagIds.Contains(command.TargetTagId))
        {
            return TagErrors.CannotMergeIntoItself;
        }

        var targetTag = await _tagRepository.GetByIdAsync(
            command.TargetTagId,
            cancellationToken);
        if (targetTag is null)
        {
            return TagErrors.TargetTagNotFound;
        }

        foreach (var sourceTagId in command.SourceTagIds)
        {
            var sourceTag = await _tagRepository.GetByIdAsync(
                sourceTagId,
                cancellationToken);
            if (sourceTag is null)
            {
                return TagErrors.SourceTagNotFound(sourceTagId);
            }

            if (sourceTag.ProjectId != targetTag.ProjectId)
            {
                return TagErrors.SourceAndTargetTagsMustBelongToSameProject;
            }

            var workItems = await _workItemRepository.ListByTagIdAsync(
                sourceTagId,
                cancellationToken);

            foreach (var workItem in workItems)
            {
                workItem.RemoveTag(sourceTag.Name);
                workItem.AddTag(targetTag);
                _workItemRepository.Update(workItem);
            }

            _tagRepository.Remove(sourceTag);
        }

        return Result.Success;
    }
}
