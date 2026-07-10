using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.Tags.Commands.DeleteTags;

public sealed record DeleteTagsCommand(
    Guid ProjectId,
    List<Guid> TagIds) : IAuthorizationRequest<ErrorOr<Success>>
{
    public Permission RequiredPermission => Permission.ProjectManageTags;
    public ResourceType ResourceType => ResourceType.Project;
    public Guid ResourceId => ProjectId;
}
