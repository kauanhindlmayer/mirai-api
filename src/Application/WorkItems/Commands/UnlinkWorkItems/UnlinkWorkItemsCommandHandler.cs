using Domain.WorkItems;
using ErrorOr;
using MediatR;

namespace Application.WorkItems.Commands.UnlinkWorkItems;

internal sealed class UnlinkWorkItemsCommandHandler
    : IRequestHandler<UnlinkWorkItemsCommand, ErrorOr<Success>>
{
    private readonly IWorkItemRepository _workItemRepository;

    public UnlinkWorkItemsCommandHandler(IWorkItemRepository workItemRepository)
    {
        _workItemRepository = workItemRepository;
    }

    public async Task<ErrorOr<Success>> Handle(
        UnlinkWorkItemsCommand command,
        CancellationToken cancellationToken)
    {
        var workItem = await _workItemRepository.GetByIdWithLinksAsync(
            command.WorkItemId,
            cancellationToken);

        if (workItem is null)
        {
            return WorkItemErrors.NotFound;
        }

        var result = workItem.RemoveLink(command.LinkId);

        if (result.IsError)
        {
            return result.Errors;
        }

        _workItemRepository.Update(workItem);

        return Result.Success;
    }
}
