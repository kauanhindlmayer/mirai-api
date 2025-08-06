using Domain.WorkItems;
using Domain.WorkItems.Enums;

namespace Domain.UnitTests.WorkItems;

public static class WorkItemFactory
{
    public static WorkItem CreateWorkItem(
        Guid? projectId = null,
        int code = 0,
        string title = "Test Work Item",
        WorkItemType type = WorkItemType.UserStory,
        Guid? assignedTeamId = null,
        Guid? sprintId = null,
        Guid? parentWorkItemId = null)
    {
        return new WorkItem(
            projectId ?? Guid.NewGuid(),
            code,
            title,
            type,
            assignedTeamId,
            sprintId,
            parentWorkItemId);
    }
}