using Domain.Common;
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
}