using Mirai.Domain.WorkItems;
using Mirai.Domain.WorkItems.Enums;
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
            code: Constants.WorkItem.Code,
            title: title,
            type: type ?? WorkItemType.UserStory);
    }
}