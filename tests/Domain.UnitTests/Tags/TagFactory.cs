using Domain.Tags;

namespace Domain.UnitTests.Tags;

public static class TagFactory
{
    public const string Name = "Tag Name";
    public const string Description = "Tag Description";
    public const string Color = "#CCCCCC";
    public static readonly Guid ProjectId = Guid.NewGuid();

    public static Tag CreateTag(
        string name = Name,
        string description = Description,
        string color = Color,
        Guid? projectId = null)
    {
        return new(name, description, color, projectId ?? ProjectId);
    }
}