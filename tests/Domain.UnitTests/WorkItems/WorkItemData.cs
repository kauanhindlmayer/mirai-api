using Domain.WorkItems;
using Domain.WorkItems.Enums;

namespace Domain.UnitTests.WorkItems;

internal static class WorkItemData
{
    public const int Code = 1;
    public const string Title = "Work Item Title";
    public const string Description = "Work Item Description";
    public const string AcceptanceCriteria = "Work Item Acceptance Criteria";
    public static readonly Guid ProjectId = Guid.NewGuid();
    public static readonly WorkItemType Type = WorkItemType.UserStory;
    public static readonly Guid AssignedTeamId = Guid.NewGuid();
    public static readonly Guid SprintId = Guid.NewGuid();
    public static readonly Guid ParentWorkItemId = Guid.NewGuid();

    public static WorkItem Create(
        Guid? projectId = null,
        int? code = null,
        string? title = null,
        WorkItemType? type = null,
        Guid? assignedTeamId = null,
        Guid? sprintId = null,
        Guid? parentWorkItemId = null)
    {
        return new WorkItem(
            projectId ?? ProjectId,
            code ?? Code,
            title ?? Title,
            type ?? Type,
            assignedTeamId ?? AssignedTeamId,
            sprintId ?? SprintId,
            parentWorkItemId ?? ParentWorkItemId);
    }
}
