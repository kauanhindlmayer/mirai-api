using System.Linq.Expressions;
using Application.WikiPages.Queries.GetWikiPage;
using Application.WikiPages.Queries.ListWikiPages;
using Domain.WikiPages;

namespace Application.WikiPages.Common;

internal static class WikiPageQueries
{
    public static Expression<Func<WikiPage, WikiPageResponse>> ProjectToDto()
    {
        return wp => new WikiPageResponse
        {
            Id = wp.Id,
            ProjectId = wp.ProjectId,
            Title = wp.Title,
            Content = wp.Content,
            Author = new AuthorResponse
            {
                Name = wp.Author.FullName,
                ImageUrl = wp.Author.ImageUrl,
            },
            Comments = wp.Comments.Select(comment => new WikiPageCommentResponse
            {
                Id = comment.Id,
                Author = new AuthorResponse
                {
                    Name = comment.Author.FullName,
                    ImageUrl = comment.Author.ImageUrl,
                },
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt,
            }),
            CreatedAt = wp.CreatedAt,
            UpdatedAt = wp.UpdatedAt,
        };
    }

    public static Expression<Func<WikiPage, WikiPageBriefResponse>> ProjectToBriefDto()
    {
        return wp => new WikiPageBriefResponse
        {
            Id = wp.Id,
            Title = wp.Title,
            Position = wp.Position,
            SubPages = wp.SubWikiPages
                .AsQueryable()
                .Select(ProjectToBriefDto()),
        };
    }
}