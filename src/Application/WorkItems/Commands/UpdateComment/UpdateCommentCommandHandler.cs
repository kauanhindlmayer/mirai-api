using Application.Abstractions.Authentication;
using Domain.WorkItems;
using ErrorOr;
using MediatR;

namespace Application.WorkItems.Commands.UpdateComment;

internal sealed class UpdateCommentCommandHandler
    : IRequestHandler<UpdateCommentCommand, ErrorOr<Success>>
{
    private readonly IWorkItemRepository _workItemRepository;
    private readonly IUserContext _userContext;

    public UpdateCommentCommandHandler(
        IWorkItemRepository workItemRepository,
        IUserContext userContext)
    {
        _workItemRepository = workItemRepository;
        _userContext = userContext;
    }

    public async Task<ErrorOr<Success>> Handle(
        UpdateCommentCommand command,
        CancellationToken cancellationToken)
    {
        var workItem = await _workItemRepository.GetByIdWithCommentsAsync(
            command.WorkItemId,
            cancellationToken);

        if (workItem is null)
        {
            return WorkItemErrors.NotFound;
        }

        var result = workItem.UpdateComment(
            command.CommentId,
            command.Content,
            _userContext.UserId);

        if (result.IsError)
        {
            return result;
        }

        _workItemRepository.Update(workItem);

        return Result.Success;
    }
}