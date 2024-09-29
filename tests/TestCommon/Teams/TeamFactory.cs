using Mirai.Domain.Teams;
using TestCommon.TestConstants;

namespace TestCommon.Teams;

public static class TeamFactory
{
    public static Team CreateTeam(
        string name = Constants.Team.Name,
        Guid? projectId = null)
    {
        return new(
            name: name,
            projectId: projectId ?? Constants.Project.Id);
    }
}