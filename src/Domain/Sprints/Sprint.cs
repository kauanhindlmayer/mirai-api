using Domain.Common;
using Domain.Teams;
using Domain.WorkItems;

namespace Domain.Sprints;

public sealed class Sprint : Entity
{
    public Guid TeamId { get; private set; }
    public Team Team { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public ICollection<WorkItem> WorkItems { get; private set; } = [];

    public Sprint(
        Guid teamId,
        string name,
        DateTime startDate,
        DateTime endDate)
    {
        TeamId = teamId;
        Name = name;
        StartDate = startDate;
        EndDate = endDate;
    }

    private Sprint()
    {
    }
}