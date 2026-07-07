using Domain.WorkItems;
using ErrorOr;
using MediatR;

namespace Application.WorkItems.Commands.RemovePullRequestLink;

internal sealed class RemovePullRequestLinkCommandHandler
    : IRequestHandler<RemovePullRequestLinkCommand, ErrorOr<Success>>
{
    private readonly IWorkItemRepository _workItemRepository;

    public RemovePullRequestLinkCommandHandler(IWorkItemRepository workItemRepository)
    {
        _workItemRepository = workItemRepository;
    }

    public async Task<ErrorOr<Success>> Handle(
        RemovePullRequestLinkCommand command,
        CancellationToken cancellationToken)
    {
        var workItem = await _workItemRepository.GetByIdWithPullRequestLinksAsync(
            command.WorkItemId,
            cancellationToken);

        if (workItem is null)
        {
            return WorkItemErrors.NotFound;
        }

        var result = workItem.RemovePullRequestLink(command.LinkId);
        if (result.IsError)
        {
            return result.Errors;
        }

        _workItemRepository.Update(workItem);

        return Result.Success;
    }
}
