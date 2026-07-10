using Application.Abstractions;
using Application.Abstractions.Authorization;
using Application.TagImportJobs.Queries.GetTagImportJob;
using Domain.Authorization;
using ErrorOr;

namespace Application.TagImportJobs.Queries.ListTagImportJobs;

public sealed record ListTagImportJobsQuery(
    Guid ProjectId,
    int Page,
    int PageSize) : IAuthorizationRequest<ErrorOr<PaginatedList<TagImportJobResponse>>>
{
    public Permission RequiredPermission => Permission.ProjectView;
    public ResourceType ResourceType => ResourceType.Project;
    public Guid ResourceId => ProjectId;
}
