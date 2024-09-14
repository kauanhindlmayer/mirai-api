using ErrorOr;
using MediatR;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.WorkItems;

namespace Mirai.Application.WorkItems.Commands.AddComment;

public class AddCommentCommandHandler(
    IWorkItemsRepository _workItemsRepository,
    ICurrentUserProvider _currentUserProvider)
    : IRequestHandler<AddCommentCommand, ErrorOr<WorkItemComment>>
{
    public async Task<ErrorOr<WorkItemComment>> Handle(
        AddCommentCommand request,
        CancellationToken cancellationToken)
    {
        var workItem = await _workItemsRepository.GetByIdAsync(
            request.WorkItemId,
            cancellationToken);

        if (workItem is null)
        {
            return Error.NotFound(description: "Work item not found");
        }

        var currentUser = _currentUserProvider.GetCurrentUser();

        var comment = new WorkItemComment(
            workItem.Id,
            currentUser.Id,
            request.Content);

        workItem.AddComment(comment);
        await _workItemsRepository.UpdateAsync(workItem, cancellationToken);

        return comment;
    }
}