using ErrorOr;
using MediatR;

namespace Application.TagImportJobs.Queries.GetTagImportJobStatus;

public sealed record GetTagImportJobStatusQuery(Guid JobImportId)
    : IRequest<ErrorOr<TagImportJobResponse>>;