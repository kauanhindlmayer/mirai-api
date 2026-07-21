using Domain.Shared;
using Domain.Teams;
using Domain.WorkItems;
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
    public ICollection<WorkItem> WorkItems { get; private set; } = [];

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
        if (WorkItems.Any(wi => wi.Id == workItem.Id))
        {
            return SprintErrors.WorkItemAlreadyInSprint;
        }

        WorkItems.Add(workItem);
        return Result.Success;
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