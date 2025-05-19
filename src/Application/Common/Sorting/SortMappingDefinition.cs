namespace Application.Common.Sorting;

public sealed class SortMappingDefinition<TSource, TDestination> : ISortMappingDefinition
{
    public required SortMapping[] Mappings { get; init; }
}
