using Ardalis.SmartEnum;

namespace Domain.WorkItems.Enums;

public class WorkItemType(string name, int value)
    : SmartEnum<WorkItemType>(name, value)
{
    public static readonly WorkItemType UserStory = new(nameof(UserStory), 0);
    public static readonly WorkItemType Bug = new(nameof(Bug), 1);
    public static readonly WorkItemType Defect = new(nameof(Defect), 2);
    public static readonly WorkItemType Epic = new(nameof(Epic), 3);
    public static readonly WorkItemType Feature = new(nameof(Feature), 4);
}