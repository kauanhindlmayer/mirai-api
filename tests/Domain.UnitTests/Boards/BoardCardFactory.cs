using Domain.Boards;

namespace Domain.UnitTests.Boards;

internal static class BoardCardFactory
{
    public const int Position = 0;
    public static readonly Guid ColumnId = Guid.NewGuid();
    public static readonly Guid WorkItemId = Guid.NewGuid();

    public static BoardCard Create(
        Guid? columnId = null,
        Guid? workItemId = null,
        int? position = null)
    {
        return new(
            columnId ?? ColumnId,
            workItemId ?? WorkItemId,
            position ?? Position);
    }
}