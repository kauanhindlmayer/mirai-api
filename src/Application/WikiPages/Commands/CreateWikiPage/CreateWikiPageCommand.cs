using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.WikiPages.Commands.CreateWikiPage;

public sealed record CreateWikiPageCommand(
    Guid ProjectId,
    string Title,
    string Content,
    Guid? ParentWikiPageId) : IAuthorizationRequest<ErrorOr<Guid>>
{
    public Permission RequiredPermission => Permission.ProjectManageWikiPages;
    public ResourceType ResourceType => ResourceType.Project;
    public Guid ResourceId => ProjectId;
}
