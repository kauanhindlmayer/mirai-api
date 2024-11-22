using Domain.Teams;

namespace Domain.UnitTests.Teams;

public static class TeamFactory
{
    public static Team CreateTeam(
        Guid? projectId = null,
        string name = "Team")
    {
        return new(
            projectId ?? Guid.NewGuid(),
            name);
    }
}