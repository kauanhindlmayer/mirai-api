using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.WikiPages.Commands.DeleteComment;

public sealed record DeleteCommentCommand(
    Guid WikiPageId,
    Guid CommentId) : IAuthorizationRequest<ErrorOr<Success>>
{
    public Permission RequiredPermission => Permission.ProjectView;
    public ResourceType ResourceType => ResourceType.WikiPage;
    public Guid ResourceId => WikiPageId;
}
