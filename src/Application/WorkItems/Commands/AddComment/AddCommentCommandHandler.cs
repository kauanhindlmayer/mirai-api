using Application.Abstractions.Authentication;
using Domain.WorkItems;
using ErrorOr;
using MediatR;

namespace Application.WorkItems.Commands.AddComment;

internal sealed class AddCommentCommandHandler
    : IRequestHandler<AddCommentCommand, ErrorOr<Guid>>
{
    private readonly IWorkItemRepository _workItemRepository;
    private readonly IUserContext _userContext;

    public AddCommentCommandHandler(
        IWorkItemRepository workItemRepository,
        IUserContext userContext)
    {
        _workItemRepository = workItemRepository;
        _userContext = userContext;
    }

    public async Task<ErrorOr<Guid>> Handle(
        AddCommentCommand command,
        CancellationToken cancellationToken)
    {
        var workItem = await _workItemRepository.GetByIdWithCommentsAsync(
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
        _workItemRepository.Update(workItem);

        return comment.Id;
    }
}