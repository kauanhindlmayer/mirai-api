using Domain.Boards;

namespace Domain.UnitTests.Boards;

internal static class BoardColumnFactory
{
    public const string Name = "Test Column";
    public const int WipLimit = 5;
    public const string DefinitionOfDone = "Definition of Done";
    public static readonly Guid BoardId = Guid.NewGuid();

    public static BoardColumn Create(
        Guid? boardId = null,
        string? name = null,
        int? wipLimit = null,
        string? definitionOfDone = null)
    {
        return new(
            boardId ?? BoardId,
            name ?? Name,
            wipLimit ?? WipLimit,
            definitionOfDone ?? DefinitionOfDone);
    }
}