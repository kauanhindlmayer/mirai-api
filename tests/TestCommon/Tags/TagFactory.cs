using Mirai.Domain.Tags;
using TestCommon.TestConstants;

namespace TestCommon.Tags;

public static class TagFactory
{
    public static Tag CreateTag(
        string name = Constants.Tag.Name)
    {
        return new(name);
    }
}