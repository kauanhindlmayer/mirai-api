using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.TagImportJobs.Queries.GetTagImportJob;

public sealed record GetTagImportJobQuery(
    Guid ProjectId,
    Guid JobImportId) : IAuthorizationRequest<ErrorOr<TagImportJobResponse>>
{
    public Permission RequiredPermission => Permission.ProjectView;
    public ResourceType ResourceType => ResourceType.Project;
    public Guid ResourceId => ProjectId;
}