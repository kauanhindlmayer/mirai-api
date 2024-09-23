namespace TestCommon.TestConstants;

public static partial class Constants
{
    public static class WorkItem
    {
        public const int Code = 1;
        public const string Title = "Test Work Item";
        public const string Description = "This is a test work item.";
        public static readonly Guid Id = Guid.NewGuid();
        public static readonly Guid ProjectId = Guid.NewGuid();
    }
}