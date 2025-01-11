using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Services;
using Domain.WorkItems;
using ErrorOr;
using MediatR;

namespace Application.WorkItems.Commands.AddComment;

internal sealed class AddCommentCommandHandler : IRequestHandler<AddCommentCommand, ErrorOr<Guid>>
{
    private readonly IWorkItemsRepository _workItemsRepository;
    private readonly IUserContext _userContext;

    public AddCommentCommandHandler(
        IWorkItemsRepository workItemsRepository,
        IUserContext userContext)
    {
        _workItemsRepository = workItemsRepository;
        _userContext = userContext;
    }

    public async Task<ErrorOr<Guid>> Handle(
        AddCommentCommand command,
        CancellationToken cancellationToken)
    {
        var workItem = await _workItemsRepository.GetByIdWithCommentsAsync(
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

        return comment.Id;
    }
}