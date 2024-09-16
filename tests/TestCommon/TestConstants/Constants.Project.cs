namespace TestCommon.TestConstants;

public static partial class Constants
{
    public static class Project
    {
        public const string Name = "Test Project";
        public const string Description = "This is a test project.";
        public static readonly Guid OrganizationId = Guid.NewGuid();
        public static readonly Guid Id = Guid.NewGuid();
    }
}