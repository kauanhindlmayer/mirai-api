using System.Linq.Expressions;
using Application.WorkItems.Queries.GetWorkItem;
using Application.WorkItems.Queries.ListWorkItems;
using Application.WorkItems.Queries.SearchWorkItems;
using Domain.WorkItems;
using Pgvector;
using Pgvector.EntityFrameworkCore;

namespace Application.WorkItems.Queries;

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
            Planning = new PlanningResponse
            {
                StoryPoints = wi.Planning.StoryPoints,
                Priority = wi.Planning.Priority,
            },
            Classification = new ClassificationResponse
            {
                ValueArea = wi.Classification.ValueArea.ToString(),
            },
            ParentWorkItem = wi.ParentWorkItem == null
                ? null
                : new RelatedWorkItemResponse
                {
                    Id = wi.ParentWorkItem.Id,
                    Code = wi.ParentWorkItem.Code,
                    Title = wi.ParentWorkItem.Title,
                    Status = wi.ParentWorkItem.Status.ToString(),
                    Type = wi.ParentWorkItem.Type.ToString(),
                    AssigneeId = wi.ParentWorkItem.AssignedUserId,
                },
            ChildWorkItems = wi.ChildWorkItems.Select(cwi => new RelatedWorkItemResponse
            {
                Id = cwi.Id,
                Code = cwi.Code,
                Title = cwi.Title,
                Status = cwi.Status.ToString(),
                Type = cwi.Type.ToString(),
                AssigneeId = cwi.AssignedUserId,
            }),
            AssigneeId = wi.AssignedUserId,
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
            Tags = wi.Tags.Select(t => new TagResponse
            {
                Id = t.Id,
                Name = t.Name,
                Color = t.Color,
            }),
            CreatedAtUtc = wi.CreatedAtUtc,
            UpdatedAtUtc = wi.UpdatedAtUtc,
        };
    }

    public static Expression<Func<WorkItem, WorkItemResponseWithDistance>> ProjectToDtoWithDistance(
        Vector vector)
    {
        return (wi) => new WorkItemResponseWithDistance
        {
            Id = wi.Id,
            Code = wi.Code,
            Title = wi.Title,
            Description = wi.Description,
            AcceptanceCriteria = wi.AcceptanceCriteria,
            Type = wi.Type.ToString(),
            CreatedAtUtc = wi.CreatedAtUtc,
            UpdatedAtUtc = wi.UpdatedAtUtc,
            Distance = wi.SearchVector.CosineDistance(vector),
        };
    }
}