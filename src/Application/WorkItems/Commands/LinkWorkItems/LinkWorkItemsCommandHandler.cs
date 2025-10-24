using Domain.WorkItems;
using ErrorOr;
using MediatR;

namespace Application.WorkItems.Commands.LinkWorkItems;

internal sealed class LinkWorkItemsCommandHandler
    : IRequestHandler<LinkWorkItemsCommand, ErrorOr<Guid>>
{
    private readonly IWorkItemRepository _workItemRepository;

    public LinkWorkItemsCommandHandler(IWorkItemRepository workItemRepository)
    {
        _workItemRepository = workItemRepository;
    }

    public async Task<ErrorOr<Guid>> Handle(
        LinkWorkItemsCommand command,
        CancellationToken cancellationToken)
    {
        var sourceWorkItem = await _workItemRepository.GetByIdAsync(
            command.SourceWorkItemId,
            cancellationToken);

        if (sourceWorkItem is null)
        {
            return WorkItemErrors.NotFound;
        }

        var targetWorkItem = await _workItemRepository.GetByIdAsync(
            command.TargetWorkItemId,
            cancellationToken);

        if (targetWorkItem is null)
        {
            return WorkItemErrors.TargetWorkItemNotFound;
        }

        var link = new WorkItemLink(
            command.SourceWorkItemId,
            command.TargetWorkItemId,
            command.LinkType,
            command.Comment);

        var result = sourceWorkItem.AddLink(link);

        if (result.IsError)
        {
            return result.Errors;
        }

        _workItemRepository.Update(sourceWorkItem);

        return link.Id;
    }
}
