using Ardalis.SmartEnum;

namespace Mirai.Domain.WorkItems;

public class WorkItemType(string name, int value)
    : SmartEnum<WorkItemType>(name, value)
{
    public static readonly WorkItemType UserStory = new(nameof(UserStory), 0);
    public static readonly WorkItemType Bug = new(nameof(Bug), 2);
    public static readonly WorkItemType Defect = new(nameof(Defect), 3);
    public static readonly WorkItemType Epic = new(nameof(Epic), 4);
    public static readonly WorkItemType Feature = new(nameof(Feature), 4);
}