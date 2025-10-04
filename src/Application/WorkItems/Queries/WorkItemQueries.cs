using System.Linq.Expressions;
using Application.WorkItems.Queries.GetWorkItem;
using Domain.WorkItems;
using Domain.WorkItems.Enums;
using Pgvector;
using Pgvector.EntityFrameworkCore;

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
                    Assignee = wi.ParentWorkItem.Assignee == null
                        ? null
                        : new AssigneeResponse
                        {
                            Id = wi.ParentWorkItem.Assignee.Id,
                            FullName = wi.ParentWorkItem.Assignee.FullName,
                            Email = wi.ParentWorkItem.Assignee.Email,
                            ImageUrl = wi.ParentWorkItem.Assignee.ImageUrl,
                        },
                },
            ChildWorkItems = wi.ChildWorkItems.Select(cwi => new RelatedWorkItemResponse
            {
                Id = cwi.Id,
                Code = cwi.Code,
                Title = cwi.Title,
                Status = cwi.Status.ToString(),
                Type = cwi.Type.ToString(),
                Assignee = cwi.Assignee == null
                    ? null
                    : new AssigneeResponse
                    {
                        Id = cwi.Assignee.Id,
                        FullName = cwi.Assignee.FullName,
                        Email = cwi.Assignee.Email,
                        ImageUrl = cwi.Assignee.ImageUrl,
                    },
            }),
            Assignee = wi.Assignee == null
                ? null
                : new AssigneeResponse
                {
                    Id = wi.Assignee.Id,
                    FullName = wi.Assignee.FullName,
                    Email = wi.Assignee.Email,
                    ImageUrl = wi.Assignee.ImageUrl,
                },
            Comments = wi.Comments.Select(comment => new WorkItemCommentResponse
            {
                Id = comment.Id,
                Author = new AuthorResponse
                {
                    Id = comment.Author.Id,
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
            OutgoingLinks = wi.OutgoingLinks.Select(l => new WorkItemLinkResponse
            {
                Id = l.Id,
                TargetWorkItem = new RelatedWorkItemResponse
                {
                    Id = l.TargetWorkItem.Id,
                    Code = l.TargetWorkItem.Code,
                    Title = l.TargetWorkItem.Title,
                    Status = l.TargetWorkItem.Status.ToString(),
                    Type = l.TargetWorkItem.Type.ToString(),
                    Assignee = l.TargetWorkItem.Assignee == null
                        ? null
                        : new AssigneeResponse
                        {
                            Id = l.TargetWorkItem.Assignee.Id,
                            FullName = l.TargetWorkItem.Assignee.FullName,
                            Email = l.TargetWorkItem.Assignee.Email,
                            ImageUrl = l.TargetWorkItem.Assignee.ImageUrl,
                        },
                },
                LinkType = l.LinkType.GetForwardName(),
                Comment = l.Comment,
            }),
            IncomingLinks = wi.IncomingLinks.Select(l => new WorkItemLinkResponse
            {
                Id = l.Id,
                TargetWorkItem = new RelatedWorkItemResponse
                {
                    Id = l.SourceWorkItem.Id,
                    Code = l.SourceWorkItem.Code,
                    Title = l.SourceWorkItem.Title,
                    Status = l.SourceWorkItem.Status.ToString(),
                    Type = l.SourceWorkItem.Type.ToString(),
                    Assignee = l.SourceWorkItem.Assignee == null
                        ? null
                        : new AssigneeResponse
                        {
                            Id = l.SourceWorkItem.Assignee.Id,
                            FullName = l.SourceWorkItem.Assignee.FullName,
                            Email = l.SourceWorkItem.Assignee.Email,
                            ImageUrl = l.SourceWorkItem.Assignee.ImageUrl,
                        },
                },
                LinkType = l.LinkType.GetReverseName(),
                Comment = l.Comment,
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