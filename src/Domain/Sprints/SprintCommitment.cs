using Domain.Shared;
using Domain.WorkItems;
using Domain.WorkItems.Enums;

namespace Domain.Sprints;

/// <summary>
/// A snapshot of one work item as it stood when its sprint started: the story
/// points it carried and the status it was in. Written once at start and never
/// changed, so committed scope stays fixed however the work item later evolves.
/// </summary>
public sealed class SprintCommitment : Entity
{
    public Guid SprintId { get; private set; }
    public Guid WorkItemId { get; private set; }
    public int? CommittedStoryPoints { get; private set; }
    public WorkItemStatus StatusAtStart { get; private set; }

    public SprintCommitment(Guid sprintId, WorkItem workItem)
    {
        SprintId = sprintId;
        WorkItemId = workItem.Id;
        CommittedStoryPoints = workItem.Planning.StoryPoints;
        StatusAtStart = workItem.Status;
    }

    private SprintCommitment()
    {
    }
}
