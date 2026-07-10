using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.WikiPages.Commands.MoveWikiPage;

public sealed record MoveWikiPageCommand(
    Guid ProjectId,
    Guid WikiPageId,
    Guid? TargetParentId,
    int TargetPosition) : IAuthorizationRequest<ErrorOr<Success>>
{
    public Permission RequiredPermission => Permission.ProjectManageWikiPages;
    public ResourceType ResourceType => ResourceType.WikiPage;
    public Guid ResourceId => WikiPageId;
}
