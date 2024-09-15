namespace TestCommon.TestConstants;

public static partial class Constants
{
    public static class WikiPage
    {
        public const string Title = "Test Wiki Page";
        public const string Content = "This is a test wiki page.";
        public static readonly Guid Id = Guid.NewGuid();
        public static readonly Guid ProjectId = Guid.NewGuid();
    }
}