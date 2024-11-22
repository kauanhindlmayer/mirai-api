using Domain.WorkItems;
using Domain.WorkItems.Enums;

namespace Domain.UnitTests.WorkItems;

public static class WorkItemFactory
{
    public static WorkItem CreateWorkItem(
        Guid? projectId = null,
        WorkItemType? type = null,
        string title = "Title")
    {
        var workItemCode = 1;

        return new(
            projectId ?? Guid.NewGuid(),
            workItemCode,
            title,
            type ?? WorkItemType.UserStory);
    }
}