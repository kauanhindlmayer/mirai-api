using System.ComponentModel;

namespace Mirai.Domain.WorkItems;

public enum WorkItemType
{
    [Description("User Story")]
    UserStory,
    Bug,
    Defect,
    Epic,
    Feature,
}