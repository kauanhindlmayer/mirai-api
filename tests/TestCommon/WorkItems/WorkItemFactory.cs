using Mirai.Domain.WorkItems;
using TestCommon.TestConstants;

namespace TestCommon.WorkItems;

public static class WorkItemFactory
{
    public static WorkItem CreateWorkItem(
        Guid? projectId = null,
        WorkItemType? type = null,
        string title = Constants.WorkItem.Title)
    {
        return new(
            projectId: projectId ?? Constants.WorkItem.ProjectId,
            type: type ?? WorkItemType.UserStory,
            title: title);
    }
}