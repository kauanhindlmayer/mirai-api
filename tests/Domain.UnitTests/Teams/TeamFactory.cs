using Domain.Teams;

namespace Domain.UnitTests.Teams;

public static class TeamFactory
{
    public const string Name = "Team";
    public const string Description = "Description";

    public static Team CreateTeam(
        Guid? projectId = null,
        string name = Name,
        string description = Description)
    {
        return new(
            projectId ?? Guid.NewGuid(),
            name,
            description);
    }
}