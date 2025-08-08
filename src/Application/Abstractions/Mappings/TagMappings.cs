using Application.Abstractions.Sorting;
using Application.Tags.Queries.ListTags;
using Domain.Tags;

namespace Application.Abstractions.Mappings;

internal static class TagMappings
{
    public static readonly SortMappingDefinition<TagResponse, Tag> SortMapping = new()
    {
        Mappings =
        [
            new SortMapping(nameof(TagResponse.Name), nameof(Tag.Name)),
            new SortMapping(nameof(TagResponse.Description), nameof(Tag.Description)),
            new SortMapping(nameof(TagResponse.Color), nameof(Tag.Color)),
        ],
    };
}