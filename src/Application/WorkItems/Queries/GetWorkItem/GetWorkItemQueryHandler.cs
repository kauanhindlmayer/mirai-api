using Application.Common.Interfaces.Persistence;
using Domain.WorkItems;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.WorkItems.Queries.GetWorkItem;

internal sealed class GetWorkItemQueryHandler : IRequestHandler<GetWorkItemQuery, ErrorOr<WorkItemResponse>>
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
            .Select(wi => new WorkItemResponse
            {
                Id = wi.Id,
                ProjectId = wi.ProjectId,
                Code = wi.Code,
                Title = wi.Title,
                Description = wi.Description,
                AcceptanceCriteria = wi.AcceptanceCriteria,
                Status = wi.Status.ToString(),
                Type = wi.Type.ToString(),
                Comments = wi.Comments.Select(c => new CommentResponse
                {
                    Id = c.Id,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                }),
                Tags = wi.Tags.Select(t => t.Name),
                CreatedAt = wi.CreatedAt,
                UpdatedAt = wi.UpdatedAt,
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (workItem is null)
        {
            return WorkItemErrors.NotFound;
        }

        return workItem;
    }
}