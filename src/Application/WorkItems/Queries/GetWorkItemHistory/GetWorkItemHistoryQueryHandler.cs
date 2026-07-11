using Application.Abstractions;
using Application.Abstractions.Mappings;
using Domain.WorkItems;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.WorkItems.Queries.GetWorkItemHistory;

internal sealed class GetWorkItemHistoryQueryHandler
    : IRequestHandler<GetWorkItemHistoryQuery, ErrorOr<PaginatedList<WorkItemChangeSetResponse>>>
{
    private readonly IApplicationDbContext _context;

    public GetWorkItemHistoryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<PaginatedList<WorkItemChangeSetResponse>>> Handle(
        GetWorkItemHistoryQuery query,
        CancellationToken cancellationToken)
    {
        var workItemExists = await _context.WorkItems
            .AnyAsync(wi => wi.Id == query.WorkItemId, cancellationToken);

        if (!workItemExists)
        {
            return WorkItemErrors.NotFound;
        }

        return await _context.WorkItemChangeSets
            .AsNoTracking()
            .Where(cs => cs.WorkItemId == query.WorkItemId)
            .OrderByDescending(cs => cs.CreatedAtUtc)
            .Select(cs => new WorkItemChangeSetResponse
            {
                Id = cs.Id,
                ChangedBy = cs.ChangedByUser == null
                    ? null
                    : new WorkItemChangeActorResponse
                    {
                        Id = cs.ChangedByUser.Id,
                        Name = cs.ChangedByUser.FirstName + " " + cs.ChangedByUser.LastName,
                    },
                SystemActor = cs.SystemActor,
                Changes = cs.Changes.Select(c => new WorkItemChangeResponse
                {
                    FieldName = c.FieldName,
                    OldValue = c.OldValue,
                    NewValue = c.NewValue,
                }).ToList(),
                CreatedAtUtc = cs.CreatedAtUtc,
            })
            .PaginatedListAsync(query.Page, query.PageSize, cancellationToken);
    }
}
