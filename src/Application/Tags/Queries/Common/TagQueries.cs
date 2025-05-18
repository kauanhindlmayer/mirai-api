using System.Linq.Expressions;
using Application.Tags.Queries.ListTags;
using Domain.Tags;

namespace Application.Tags.Queries.Common;

internal static class TagQueries
{
    public static Expression<Func<Tag, TagResponse>> ProjectToDto()
    {
        return t => new TagResponse
        {
            Id = t.Id,
            Name = t.Name,
            Description = t.Description,
            Color = t.Color,
            WorkItemsCount = t.WorkItems.Count,
        };
    }
}