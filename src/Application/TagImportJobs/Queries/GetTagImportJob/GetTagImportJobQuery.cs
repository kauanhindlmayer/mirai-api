using ErrorOr;
using MediatR;

namespace Application.TagImportJobs.Queries.GetTagImportJob;

public sealed record GetTagImportJobQuery(Guid JobImportId) : IRequest<ErrorOr<TagImportJobResponse>>;