using Domain.Boards;

namespace Domain.UnitTests.Boards;

public static class BoardFactory
{
    public const string Name = "Test Board";
    public static readonly Guid TeamId = Guid.NewGuid();

    public static Board Create(
        Guid? teamId = null,
        string? name = null)
    {
        return new(
            teamId ?? TeamId,
            name ?? Name);
    }
}