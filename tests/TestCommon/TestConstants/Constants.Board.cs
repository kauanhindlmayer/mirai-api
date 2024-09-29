namespace TestCommon.TestConstants;

public static partial class Constants
{
    public static class Board
    {
        public const string Name = "Development Sprint Board";
        public const string Description = "Board for tracking tasks and progress during development sprints.";
        public const string ColumnName = "In Progress";
        public const int WipLimit = 5;
        public const string DefinitionOfDone = "All acceptance criteria met and code reviewed.";
        public static readonly Guid Id = Guid.NewGuid();
    }
}
