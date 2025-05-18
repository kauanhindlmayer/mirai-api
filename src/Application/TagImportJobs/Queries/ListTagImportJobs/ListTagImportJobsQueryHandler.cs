using Application.Common;
using Application.Common.Interfaces.Persistence;
using Application.Common.Mappings;
using Application.TagImportJobs.Queries.Common;
using Application.TagImportJobs.Queries.GetTagImportJob;
using ErrorOr;
using MediatR;

namespace Application.TagImportJobs.Queries.ListTagImportJobs;

internal sealed class ListTagImportJobsQueryHandler
    : IRequestHandler<ListTagImportJobsQuery, ErrorOr<PaginatedList<TagImportJobResponse>>>
{
    private readonly IApplicationDbContext _context;

    public ListTagImportJobsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<PaginatedList<TagImportJobResponse>>> Handle(
        ListTagImportJobsQuery query,
        CancellationToken cancellationToken)
    {
        var tagImportJobs = await _context.TagImportJobs
            .Where(job => job.ProjectId == query.ProjectId)
            .Select(TagImportJobQueries.ProjectToDto())
            .PaginatedListAsync(
                query.Page,
                query.PageSize,
                cancellationToken);

        return tagImportJobs;
    }
}