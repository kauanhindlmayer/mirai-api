namespace TestCommon.TestConstants;

public static partial class Constants
{
    public static class Organization
    {
        public const string Name = "Test Organization";
        public const string Description = "This is a test organization.";
        public const string UpdatedName = "Updated Test Organization";
        public const string UpdatedDescription = "This is an updated test organization.";
        public static readonly Guid Id = Guid.NewGuid();
    }
}