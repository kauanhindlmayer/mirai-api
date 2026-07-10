using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.Tags.Commands.CreateTag;

public sealed record CreateTagCommand(
    Guid ProjectId,
    string Name,
    string Description,
    string Color) : IAuthorizationRequest<ErrorOr<Guid>>
{
    public Permission RequiredPermission => Permission.ProjectManageTags;
    public ResourceType ResourceType => ResourceType.Project;
    public Guid ResourceId => ProjectId;
}
