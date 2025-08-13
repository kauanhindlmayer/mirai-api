using Domain.Tags;

namespace Domain.UnitTests.Tags;

public static class TagFactory
{
    public const string Name = "Tag Name";
    public const string Description = "Tag Description";
    public const string Color = "#CCCCCC";
    public static readonly Guid ProjectId = Guid.NewGuid();

    public static Tag Create(
        string? name = null,
        string? description = null,
        string? color = null,
        Guid? projectId = null)
    {
        return new(
            name ?? Name,
            description ?? Description,
            color ?? Color,
            projectId ?? ProjectId);
    }
}