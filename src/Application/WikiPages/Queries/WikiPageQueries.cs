using System.Linq.Expressions;
using Application.WikiPages.Queries.GetWikiPage;
using Domain.WikiPages;

namespace Application.WikiPages.Queries;

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
                CreatedAtUtc = comment.CreatedAtUtc,
                UpdatedAtUtc = comment.UpdatedAtUtc,
            }),
            CreatedAtUtc = wp.CreatedAtUtc,
            UpdatedAtUtc = wp.UpdatedAtUtc,
        };
    }
}