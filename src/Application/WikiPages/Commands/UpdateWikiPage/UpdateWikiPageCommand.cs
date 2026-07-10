using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.WikiPages.Commands.UpdateWikiPage;

public sealed record UpdateWikiPageCommand(
    Guid WikiPageId,
    string Title,
    string Content) : IAuthorizationRequest<ErrorOr<Guid>>
{
    public Permission RequiredPermission => Permission.ProjectManageWikiPages;
    public ResourceType ResourceType => ResourceType.WikiPage;
    public Guid ResourceId => WikiPageId;
}
