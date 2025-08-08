using Application.Abstractions;
using Domain.WorkItems;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.WorkItems.Queries.GetWorkItem;

internal sealed class GetWorkItemQueryHandler
    : IRequestHandler<GetWorkItemQuery, ErrorOr<WorkItemResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetWorkItemQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<WorkItemResponse>> Handle(
        GetWorkItemQuery query,
        CancellationToken cancellationToken)
    {
        var workItem = await _context.WorkItems
            .AsNoTracking()
            .Where(wi => wi.Id == query.WorkItemId)
            .Select(WorkItemQueries.ProjectToDto())
            .FirstOrDefaultAsync(cancellationToken);

        if (workItem is null)
        {
            return WorkItemErrors.NotFound;
        }

        return workItem;
    }
}