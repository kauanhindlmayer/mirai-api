using Mirai.Domain.Projects;
using TestCommon.Organizations;
using TestCommon.Projects;

namespace Mirai.Domain.UnitTests.Organizations;

public class OrganizationTests
{
    [Fact]
    public void AddProject_WhenProjectWithSameNameAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var organization = OrganizationFactory.CreateOrganization();
        var project = ProjectFactory.CreateProject();
        organization.AddProject(project);

        // Act
        var result = organization.AddProject(project);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Should().BeEquivalentTo(ProjectErrors.ProjectWithSameNameAlreadyExists);
    }

    [Fact]
    public void AddProject_WhenProjectWithSameNameDoesNotExists_ShouldAddProject()
    {
        // Arrange
        var organization = OrganizationFactory.CreateOrganization();
        var project = ProjectFactory.CreateProject();

        // Act
        var result = organization.AddProject(project);

        // Assert
        result.IsError.Should().BeFalse();
        organization.Projects.Should().HaveCount(1);
        organization.Projects.First().Should().BeEquivalentTo(project);
    }
}