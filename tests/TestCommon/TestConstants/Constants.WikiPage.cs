namespace TestCommon.TestConstants;

public static partial class Constants
{
    public static class WikiPage
    {
        public const string Title = "Test Wiki Page";
        public const string Content = "This is a test wiki page.";
        public const string UpdatedTitle = "Updated Test Wiki Page";
        public const string UpdatedContent = "This is an updated test wiki page.";
        public static readonly Guid Id = Guid.NewGuid();
        public static readonly Guid ProjectId = Guid.NewGuid();
    }
}