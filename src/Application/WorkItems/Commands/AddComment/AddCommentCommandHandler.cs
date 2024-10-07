using Application.Common.Interfaces;
using Domain.WorkItems;
using ErrorOr;
using MediatR;

namespace Application.WorkItems.Commands.AddComment;

public class AddCommentCommandHandler(
    IWorkItemsRepository _workItemsRepository,
    ICurrentUserProvider _currentUserProvider)
    : IRequestHandler<AddCommentCommand, ErrorOr<WorkItemComment>>
{
    public async Task<ErrorOr<WorkItemComment>> Handle(
        AddCommentCommand command,
        CancellationToken cancellationToken)
    {
        var workItem = await _workItemsRepository.GetByIdAsync(
            command.WorkItemId,
            cancellationToken);

        if (workItem is null)
        {
            return WorkItemErrors.NotFound;
        }

        var currentUser = _currentUserProvider.GetCurrentUser();

        var comment = new WorkItemComment(
            workItem.Id,
            currentUser.Id,
            command.Content);

        workItem.AddComment(comment);
        _workItemsRepository.Update(workItem);

        return comment;
    }
}