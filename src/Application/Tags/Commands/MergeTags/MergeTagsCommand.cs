using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.Tags.Commands.MergeTags;

public sealed record MergeTagsCommand(
    Guid ProjectId,
    Guid TargetTagId,
    List<Guid> SourceTagIds) : IAuthorizationRequest<ErrorOr<Success>>
{
    public Permission RequiredPermission => Permission.ProjectManageTags;
    public ResourceType ResourceType => ResourceType.Project;
    public Guid ResourceId => ProjectId;
}
