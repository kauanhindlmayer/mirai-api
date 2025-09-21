namespace Domain.WorkItems.Enums;

public enum WorkItemStatus
{
    New = 1,
    Active = 2,        // Renamed from InProgress to match Azure DevOps
    Resolved = 3,      // Moved before Closed to match workflow order
    Closed = 4,
    Removed = 5,       // Removed Reopened - use Active instead
}
