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
            CreatedAtUtc = wi.CreatedAtUtc,
            UpdatedAtUtc = wi.UpdatedAtUtc,
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
            Planning = wi.Planning.StoryPoints == null && wi.Planning.Priority == null
                ? null
                : new PlanningResponse
                {
                    StoryPoints = wi.Planning.StoryPoints,
                    Priority = wi.Planning.Priority,
                },
            Classification = wi.Classification.ValueArea == default
                ? null
                : new ClassificationResponse
                {
                    ValueArea = wi.Classification.ValueArea.ToString(),
                },
            AssigneeId = wi.AssigneeId,
            Comments = wi.Comments.Select(comment => new WorkItemCommentResponse
            {
                Id = comment.Id,
                Author = new AuthorResponse
                {
                    Name = comment.Author.FullName,
                    ImageUrl = comment.Author.ImageUrl,
                },
                Content = comment.Content,
                CreatedAtUtc = comment.CreatedAtUtc,
                UpdatedAtUtc = comment.UpdatedAtUtc,
            }),
            Tags = wi.Tags.Select(t => t.Name),
            CreatedAtUtc = wi.CreatedAtUtc,
            UpdatedAtUtc = wi.UpdatedAtUtc,
        };
    }
}