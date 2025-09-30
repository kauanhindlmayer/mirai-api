using System.Linq.Expressions;
using Domain.WorkItems;
using Pgvector;
using Pgvector.EntityFrameworkCore;
using GetWorkItemAssigneeResponse = Application.WorkItems.Queries.GetWorkItem.AssigneeResponse;

namespace Application.WorkItems.Queries;

internal static class WorkItemQueries
{
    public static Expression<Func<WorkItem, ListWorkItems.WorkItemBriefResponse>> ProjectToBriefDto()
    {
        return wi => new ListWorkItems.WorkItemBriefResponse
        {
            Id = wi.Id,
            Code = wi.Code,
            Title = wi.Title,
            Status = wi.Status.ToString(),
            Type = wi.Type.ToString(),
            Assignee = wi.Assignee == null
                ? null
                : new ListWorkItems.AssigneeResponse
                {
                    Id = wi.Assignee.Id,
                    Name = wi.Assignee.FullName,
                    ImageUrl = wi.Assignee.ImageUrl,
                },
            Tags = wi.Tags.Select(t => new ListWorkItems.TagBriefResponse
            {
                Id = t.Id,
                Name = t.Name,
                Color = t.Color,
            }),
            CreatedAtUtc = wi.CreatedAtUtc,
            UpdatedAtUtc = wi.UpdatedAtUtc,
        };
    }

    public static Expression<Func<WorkItem, GetWorkItem.WorkItemResponse>> ProjectToDto()
    {
        return wi => new GetWorkItem.WorkItemResponse
        {
            Id = wi.Id,
            ProjectId = wi.ProjectId,
            Code = wi.Code,
            Title = wi.Title,
            Description = wi.Description,
            AcceptanceCriteria = wi.AcceptanceCriteria,
            Status = wi.Status.ToString(),
            Type = wi.Type.ToString(),
            Planning = new GetWorkItem.PlanningResponse
            {
                StoryPoints = wi.Planning.StoryPoints,
                Priority = wi.Planning.Priority,
            },
            Classification = new GetWorkItem.ClassificationResponse
            {
                ValueArea = wi.Classification.ValueArea.ToString(),
            },
            ParentWorkItem = wi.ParentWorkItem == null
                ? null
                : new GetWorkItem.RelatedWorkItemResponse
                {
                    Id = wi.ParentWorkItem.Id,
                    Code = wi.ParentWorkItem.Code,
                    Title = wi.ParentWorkItem.Title,
                    Status = wi.ParentWorkItem.Status.ToString(),
                    Type = wi.ParentWorkItem.Type.ToString(),
                    AssigneeId = wi.ParentWorkItem.AssigneeId,
                    Assignee = wi.ParentWorkItem.Assignee == null
                        ? null
                        : new GetWorkItemAssigneeResponse
                        {
                            Id = wi.ParentWorkItem.Assignee.Id,
                            FullName = wi.ParentWorkItem.Assignee.FullName,
                            Email = wi.ParentWorkItem.Assignee.Email,
                            ImageUrl = wi.ParentWorkItem.Assignee.ImageUrl,
                        },
                },
            ChildWorkItems = wi.ChildWorkItems.Select(cwi => new GetWorkItem.RelatedWorkItemResponse
            {
                Id = cwi.Id,
                Code = cwi.Code,
                Title = cwi.Title,
                Status = cwi.Status.ToString(),
                Type = cwi.Type.ToString(),
                AssigneeId = cwi.AssigneeId,
                Assignee = cwi.Assignee == null
                    ? null
                    : new GetWorkItemAssigneeResponse
                    {
                        Id = cwi.Assignee.Id,
                        FullName = cwi.Assignee.FullName,
                        Email = cwi.Assignee.Email,
                        ImageUrl = cwi.Assignee.ImageUrl,
                    },
            }),
            AssigneeId = wi.AssigneeId,
            Assignee = wi.Assignee == null
                ? null
                : new GetWorkItemAssigneeResponse
                {
                    Id = wi.Assignee.Id,
                    FullName = wi.Assignee.FullName,
                    Email = wi.Assignee.Email,
                    ImageUrl = wi.Assignee.ImageUrl,
                },
            Comments = wi.Comments.Select(comment => new GetWorkItem.WorkItemCommentResponse
            {
                Id = comment.Id,
                Author = new GetWorkItem.AuthorResponse
                {
                    Id = comment.Author.Id,
                    Name = comment.Author.FullName,
                    ImageUrl = comment.Author.ImageUrl,
                },
                Content = comment.Content,
                CreatedAtUtc = comment.CreatedAtUtc,
                UpdatedAtUtc = comment.UpdatedAtUtc,
            }),
            Tags = wi.Tags.Select(t => new GetWorkItem.TagResponse
            {
                Id = t.Id,
                Name = t.Name,
                Color = t.Color,
            }),
            CreatedAtUtc = wi.CreatedAtUtc,
            UpdatedAtUtc = wi.UpdatedAtUtc,
        };
    }

    public static Expression<Func<WorkItem, SearchWorkItems.WorkItemResponseWithDistance>> ProjectToDtoWithDistance(
        Vector vector)
    {
        return (wi) => new SearchWorkItems.WorkItemResponseWithDistance
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