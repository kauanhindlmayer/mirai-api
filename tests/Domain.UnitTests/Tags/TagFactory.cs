using Domain.Tags;

namespace Domain.UnitTests.Tags;

public static class TagFactory
{
    public static Tag CreateTag(string name = "Tag Name")
    {
        return new(name);
    }
}