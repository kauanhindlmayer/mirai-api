using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.WikiPages.Commands.AddComment;

public sealed record AddCommentCommand(
    Guid WikiPageId,
    string Content) : IAuthorizationRequest<ErrorOr<Guid>>
{
    public Permission RequiredPermission => Permission.ProjectView;
    public ResourceType ResourceType => ResourceType.WikiPage;
    public Guid ResourceId => WikiPageId;
}
