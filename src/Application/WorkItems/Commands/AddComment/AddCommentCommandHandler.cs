using Application.Common.Interfaces;
using Domain.WorkItems;
using ErrorOr;
using MediatR;

namespace Application.WorkItems.Commands.AddComment;

public class AddCommentCommandHandler(
    IWorkItemsRepository _workItemsRepository,
    IUserContext _userContext)
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

        var comment = new WorkItemComment(
            workItem.Id,
            _userContext.UserId,
            command.Content);

        workItem.AddComment(comment);
        _workItemsRepository.Update(workItem);

        return comment;
    }
}