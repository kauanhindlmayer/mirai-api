using Domain.Teams;

namespace Domain.UnitTests.Teams;

public static class TeamFactory
{
    public const string Name = "Team";
    public const string Description = "Description";
    public static readonly Guid ProjectId = Guid.NewGuid();

    public static Team Create(
        Guid? projectId = null,
        string? name = null,
        string? description = null)
    {
        return new(
            projectId ?? ProjectId,
            name ?? Name,
            description ?? Description);
    }
}