using Application.WorkItems.Queries.Common;

namespace Application.WisdomExtractor.Queries.ExtractWisdom;

public sealed class WisdomResponse
{
    public required string Answer { get; init; }
    public required IEnumerable<WorkItemResponseWithDistance> Sources { get; init; }
}