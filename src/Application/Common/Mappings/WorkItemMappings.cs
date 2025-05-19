using Application.Common.Sorting;
using Application.WorkItems.Queries.Common;
using Domain.WorkItems;

namespace Application.Common.Mappings;

internal static class WorkItemMappings
{
    public static readonly SortMappingDefinition<WorkItemBriefResponse, WorkItem> SortMapping = new()
    {
        Mappings =
        [
            new SortMapping(nameof(WorkItemBriefResponse.Code), nameof(WorkItem.Code)),
            new SortMapping(nameof(WorkItemBriefResponse.Title), nameof(WorkItem.Title)),
            new SortMapping(nameof(WorkItemBriefResponse.Status), nameof(WorkItem.Status)),
            new SortMapping(nameof(WorkItemBriefResponse.Type), nameof(WorkItem.Type)),
            new SortMapping(nameof(WorkItemBriefResponse.CreatedAt), nameof(WorkItem.CreatedAt)),
            new SortMapping(nameof(WorkItemBriefResponse.UpdatedAt), nameof(WorkItem.UpdatedAt)),
        ],
    };
}
