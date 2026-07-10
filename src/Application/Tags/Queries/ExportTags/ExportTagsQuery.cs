using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.Tags.Queries.ExportTags;

public sealed record ExportTagsQuery(Guid ProjectId) : IAuthorizationRequest<ErrorOr<byte[]>>
{
    public Permission RequiredPermission => Permission.ProjectView;
    public ResourceType ResourceType => ResourceType.Project;
    public Guid ResourceId => ProjectId;
}
