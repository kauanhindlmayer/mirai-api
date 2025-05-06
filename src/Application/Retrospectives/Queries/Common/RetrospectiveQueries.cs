using System.Linq.Expressions;
using Application.Retrospectives.Queries.GetRetrospective;
using Application.Retrospectives.Queries.ListRetrospectives;
using Domain.Retrospectives;

namespace Application.Retrospectives.Queries.Common;

internal static class RetrospectiveQueries
{
    public static Expression<Func<Retrospective, RetrospectiveResponse>> ProjectToDto()
    {
        return r => new RetrospectiveResponse
        {
            Id = r.Id,
            Title = r.Title,
            MaxVotesPerUser = r.MaxVotesPerUser,
            Columns = r.Columns.Select(c => new RetrospectiveColumnResponse
            {
                Id = c.Id,
                Title = c.Title,
                Position = c.Position,
                Items = c.Items.Select(i => new RetrospectiveItemResponse
                {
                    Id = i.Id,
                    Content = i.Content,
                    Position = i.Position,
                    AuthorId = i.AuthorId,
                    Votes = i.Votes,
                    CreatedAt = i.CreatedAt,
                }),
            }),
        };
    }

    public static Expression<Func<Retrospective, RetrospectiveBriefResponse>> ProjectToBriefDto()
    {
        return r => new RetrospectiveBriefResponse
        {
            Id = r.Id,
            Title = r.Title,
        };
    }
}