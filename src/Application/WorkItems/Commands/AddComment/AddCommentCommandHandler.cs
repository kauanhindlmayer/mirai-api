using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Services;
using Domain.WorkItems;
using ErrorOr;
using MediatR;

namespace Application.WorkItems.Commands.AddComment;

internal sealed class AddCommentCommandHandler(
    IWorkItemsRepository workItemsRepository,
    IUserContext userContext)
    : IRequestHandler<AddCommentCommand, ErrorOr<WorkItemComment>>
{
    public async Task<ErrorOr<WorkItemComment>> Handle(
        AddCommentCommand command,
        CancellationToken cancellationToken)
    {
        var workItem = await workItemsRepository.GetByIdAsync(
            command.WorkItemId,
            cancellationToken);

        if (workItem is null)
        {
            return WorkItemErrors.NotFound;
        }

        var comment = new WorkItemComment(
            workItem.Id,
            userContext.UserId,
            command.Content);

        workItem.AddComment(comment);
        workItemsRepository.Update(workItem);

        return comment;
    }
}