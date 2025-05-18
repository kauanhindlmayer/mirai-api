using Application.Common.Interfaces.Persistence;
using Application.TagImportJobs.Queries.Common;
using Domain.Tags;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.TagImportJobs.Queries.GetTagImportJobStatus;

internal sealed class GetTagImportJobStatusQueryHandler
    : IRequestHandler<GetTagImportJobStatusQuery, ErrorOr<TagImportJobResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetTagImportJobStatusQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<TagImportJobResponse>> Handle(
        GetTagImportJobStatusQuery query,
        CancellationToken cancellationToken)
    {
        var importJob = await _context.TagImportJobs
            .AsNoTracking()
            .Where(job => job.Id == query.JobImportId)
            .Select(TagImportJobQueries.ProjectToDto())
            .FirstOrDefaultAsync(cancellationToken);

        if (importJob is null)
        {
            return TagImportJobErrors.NotFound;
        }

        return importJob;
    }
}