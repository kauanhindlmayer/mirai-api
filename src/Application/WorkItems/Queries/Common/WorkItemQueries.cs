using System.Linq.Expressions;
using Application.WorkItems.Queries.GetWorkItem;
using Domain.WorkItems;

namespace Application.WorkItems.Queries.Common;

internal static class WorkItemQueries
{
    public static Expression<Func<WorkItem, WorkItemBriefResponse>> ProjectToBriefDto()
    {
        return wi => new WorkItemBriefResponse
        {
            Id = wi.Id,
            Code = wi.Code,
            Title = wi.Title,
            Status = wi.Status.ToString(),
            Type = wi.Type.ToString(),
            Tags = wi.Tags.Select(t => new TagBriefResponse
            {
                Id = t.Id,
                Name = t.Name,
                Color = t.Color,
            }),
            CreatedAt = wi.CreatedAt,
            UpdatedAt = wi.UpdatedAt,
        };
    }

    public static Expression<Func<WorkItem, WorkItemResponse>> ProjectToDto()
    {
        return wi => new WorkItemResponse
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
        };
    }
}