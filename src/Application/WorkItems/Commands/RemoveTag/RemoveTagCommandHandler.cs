using Domain.WorkItems;
using ErrorOr;
using MediatR;

namespace Application.WorkItems.Commands.RemoveTag;

internal sealed class RemoveTagCommandHandler
    : IRequestHandler<RemoveTagCommand, ErrorOr<Success>>
{
    private readonly IWorkItemsRepository _workItemsRepository;

    public RemoveTagCommandHandler(
        IWorkItemsRepository workItemsRepository)
    {
        _workItemsRepository = workItemsRepository;
    }

    public async Task<ErrorOr<Success>> Handle(
        RemoveTagCommand command,
        CancellationToken cancellationToken)
    {
        var workItem = await _workItemsRepository.GetByIdWithTagsAsync(
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