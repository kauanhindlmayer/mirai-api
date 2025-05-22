using Application.Common;
using Application.TagImportJobs.Queries.GetTagImportJob;
using ErrorOr;
using MediatR;

namespace Application.TagImportJobs.Queries.ListTagImportJobs;

public sealed record ListTagImportJobsQuery(
    Guid ProjectId,
    int Page,
    int PageSize) : IRequest<ErrorOr<PaginatedList<TagImportJobResponse>>>;