using Contracts.Projects;

namespace WebApi.FunctionalTests.Projects;

public static class ProjectRequestFactory
{
    public static CreateProjectRequest CreateCreateProjectRequest(
        string name = "Project Name",
        string description = "Project Description")
    {
        return new CreateProjectRequest
        {
            Name = name,
            Description = description,
        };
    }
}