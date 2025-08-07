using Application.Abstractions;
using Domain.TagImportJobs;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.TagImportJobs.Queries.GetTagImportJob;

internal sealed class GetTagImportJobQueryHandler
    : IRequestHandler<GetTagImportJobQuery, ErrorOr<TagImportJobResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetTagImportJobQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<TagImportJobResponse>> Handle(
        GetTagImportJobQuery query,
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