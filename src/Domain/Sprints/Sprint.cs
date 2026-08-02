using Domain.Shared;
using Domain.Teams;
using Domain.WorkItems;
using Domain.WorkItems.Enums;
using ErrorOr;

namespace Domain.Sprints;

public sealed class Sprint : Entity
{
    public Guid TeamId { get; private set; }
    public Team Team { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }
    public SprintStatus Status { get; private set; } = SprintStatus.Planned;
    public DateTime? StartedAtUtc { get; private set; }
    public DateTime? CompletedAtUtc { get; private set; }
    public int? CompletedStoryPoints { get; private set; }
    public int? CompletedWorkItemCount { get; private set; }
    public ICollection<WorkItem> WorkItems { get; private set; } = [];
    public ICollection<SprintCommitment> Commitments { get; private set; } = [];

    public int CommittedStoryPoints =>
        Commitments.Sum(c => c.CommittedStoryPoints ?? 0);

    public int CommittedWorkItemCount => Commitments.Count;

    public Sprint(
        Guid teamId,
        string name,
        DateOnly startDate,
        DateOnly endDate)
    {
        TeamId = teamId;
        Name = name;
        StartDate = startDate;
        EndDate = endDate;
    }

    private Sprint()
    {
    }

    public ErrorOr<Success> AddWorkItem(WorkItem workItem)
    {
        if (Status == SprintStatus.Completed)
        {
            return SprintErrors.IsCompleted;
        }

        if (WorkItems.Any(wi => wi.Id == workItem.Id))
        {
            return SprintErrors.WorkItemAlreadyInSprint;
        }

        WorkItems.Add(workItem);
        return Result.Success;
    }

    public void RemoveWorkItem(WorkItem workItem)
    {
        WorkItems.Remove(workItem);
    }

    /// <summary>
    /// The work still in flight when the sprint is being completed. Closed items
    /// are delivered and Removed items are abandoned - both are terminal and stay
    /// in the sprint as its record, so neither is offered for carry-over.
    /// </summary>
    public IReadOnlyList<WorkItem> IncompleteWorkItems()
    {
        return WorkItems
            .Where(wi => wi.Status != WorkItemStatus.Closed
                && wi.Status != WorkItemStatus.Removed)
            .ToList();
    }

    public void Update(string name, DateOnly startDate, DateOnly endDate)
    {
        Name = name;
        StartDate = startDate;
        EndDate = endDate;
    }

    /// <summary>
    /// Starting is a deliberate act, not a date passing - a sprint is Active
    /// because someone started it, which is also the moment its committed scope
    /// is fixed.
    /// </summary>
    public void Start()
    {
        Status = SprintStatus.Active;
        StartedAtUtc = DateTime.UtcNow;
        foreach (var workItem in WorkItems)
        {
            Commitments.Add(new SprintCommitment(Id, workItem));
        }
    }

    /// <summary>
    /// Freezes the delivered figures and marks the sprint Completed. Delivered is
    /// measured over committed items only - work added mid-sprint is scope change,
    /// not commitment delivery - and at each item's committed points, so later
    /// re-estimation cannot distort a finished sprint's numbers. Delivered means
    /// Closed, the project-wide definition velocity also uses.
    /// </summary>
    public void Complete()
    {
        var committedPointsByItem = Commitments
            .ToDictionary(c => c.WorkItemId, c => c.CommittedStoryPoints);
        var deliveredCommitted = WorkItems
            .Where(wi => wi.Status == WorkItemStatus.Closed
                && committedPointsByItem.ContainsKey(wi.Id))
            .ToList();
        CompletedWorkItemCount = deliveredCommitted.Count;
        CompletedStoryPoints = deliveredCommitted.Sum(wi => committedPointsByItem[wi.Id] ?? 0);
        Status = SprintStatus.Completed;
        CompletedAtUtc = DateTime.UtcNow;
    }

    public void ReturnWorkItemsToBacklog()
    {
        foreach (var workItem in WorkItems)
        {
            workItem.RemoveFromSprint();
        }

        WorkItems.Clear();
    }

    /// <summary>
    /// A sprint's date range includes both endpoints, so a sprint ending on the
    /// 14th and one starting on the 15th are back-to-back, not overlapping.
    /// </summary>
    public bool Overlaps(DateOnly startDate, DateOnly endDate)
    {
        return StartDate <= endDate && startDate <= EndDate;
    }
}