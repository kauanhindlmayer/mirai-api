using Domain.Organizations;
using Domain.Organizations.Events;
using Domain.Projects;
using Domain.UnitTests.Infrastructure;
using Domain.UnitTests.Projects;
using Domain.UnitTests.Users;
using Domain.Users;

namespace Domain.UnitTests.Organizations;

public class OrganizationTests : BaseTest
{
    [Fact]
    public void CreateOrganization_ShouldRaiseOrganizationCreatedDomainEvent()
    {
        // Act
        var organization = OrganizationFactory.CreateOrganization();

        // Assert
        var domainEvent = AssertDomainEventWasPublished<OrganizationCreatedDomainEvent>(organization);
        domainEvent.Organization.Should().Be(organization);
    }

    [Fact]
    public void CreateOrganization_ShouldSetProperties()
    {
        // Act
        var organization = OrganizationFactory.CreateOrganization();

        // Assert
        organization.Name.Should().Be(OrganizationFactory.Name);
        organization.Description.Should().Be(OrganizationFactory.Description);
    }

    [Fact]
    public void UpdateOrganization_ShouldRaiseOrganizationUpdatedDomainEvent()
    {
        // Arrange
        var organization = OrganizationFactory.CreateOrganization();

        // Act
        organization.Update("New Name", "New Description");

        // Assert
        var domainEvent = AssertDomainEventWasPublished<OrganizationUpdatedDomainEvent>(organization);
        domainEvent.Organization.Should().Be(organization);
    }

    [Fact]
    public void UpdateOrganization_ShouldUpdateProperties()
    {
        // Arrange
        var organization = OrganizationFactory.CreateOrganization();

        // Act
        organization.Update("New Name", "New Description");

        // Assert
        organization.Name.Should().Be("New Name");
        organization.Description.Should().Be("New Description");
    }

    [Fact]
    public void DeleteOrganization_ShouldRaiseOrganizationDeletedDomainEvent()
    {
        // Arrange
        var organization = OrganizationFactory.CreateOrganization();

        // Act
        organization.Delete();

        // Assert
        var domainEvent = AssertDomainEventWasPublished<OrganizationDeletedDomainEvent>(organization);
        domainEvent.Organization.Should().Be(organization);
    }

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
        result.FirstError.Should().BeEquivalentTo(ProjectErrors.AlreadyExists);
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

    [Fact]
    public void RemoveProject_WhenProjectDoesNotExists_ShouldReturnError()
    {
        // Arrange
        var organization = OrganizationFactory.CreateOrganization();
        var project = ProjectFactory.CreateProject();

        // Act
        var result = organization.RemoveProject(project);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.FirstError.Should().BeEquivalentTo(ProjectErrors.NotFound);
    }

    [Fact]
    public void RemoveProject_WhenProjectExists_ShouldRemoveProject()
    {
        // Arrange
        var organization = OrganizationFactory.CreateOrganization();
        var project = ProjectFactory.CreateProject();
        organization.AddProject(project);

        // Act
        var result = organization.RemoveProject(project);

        // Assert
        result.IsError.Should().BeFalse();
        organization.Projects.Should().BeEmpty();
    }

    [Fact]
    public void AddUser_WhenUserDoesNotExist_ShouldAddUser()
    {
        // Arrange
        var organization = OrganizationFactory.CreateOrganization();
        var user = UserFactory.CreateUser();

        // Act
        var result = organization.AddUser(user);

        // Assert
        result.IsError.Should().BeFalse();
        organization.Users.Should().HaveCount(1);
        organization.Users.First().Should().Be(user);
    }

    [Fact]
    public void AddUser_WhenUserDoesNotExist_ShouldRaiseUserAddedToOrganizationDomainEvent()
    {
        // Arrange
        var organization = OrganizationFactory.CreateOrganization();
        var user = UserFactory.CreateUser();

        // Act
        organization.AddUser(user);

        // Assert
        var domainEvent = AssertDomainEventWasPublished<UserAddedToOrganizationDomainEvent>(organization);
        domainEvent.Organization.Should().Be(organization);
        domainEvent.User.Should().Be(user);
    }

    [Fact]
    public void AddUser_WhenUserAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var organization = OrganizationFactory.CreateOrganization();
        var user = UserFactory.CreateUser();
        organization.AddUser(user);

        // Act
        var result = organization.AddUser(user);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(OrganizationErrors.UserAlreadyExists);
        organization.Users.Should().HaveCount(1);
    }

    [Fact]
    public void RemoveUser_WhenUserDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var organization = OrganizationFactory.CreateOrganization();
        var userId = Guid.NewGuid();

        // Act
        var result = organization.RemoveUser(userId);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(UserErrors.NotFound);
    }

    [Fact]
    public void RemoveUser_WhenUserExists_ShouldRemoveUser()
    {
        // Arrange
        var organization = OrganizationFactory.CreateOrganization();
        var user = UserFactory.CreateUser();
        organization.AddUser(user);

        // Act
        var result = organization.RemoveUser(user.Id);

        // Assert
        result.IsError.Should().BeFalse();
        organization.Users.Should().BeEmpty();
    }

    [Fact]
    public void RemoveUser_WhenUserExists_ShouldRaiseUserRemovedFromOrganizationDomainEvent()
    {
        // Arrange
        var organization = OrganizationFactory.CreateOrganization();
        var user = UserFactory.CreateUser();
        organization.AddUser(user);

        // Act
        organization.RemoveUser(user.Id);

        // Assert
        var domainEvent = AssertDomainEventWasPublished<UserRemovedFromOrganizationDomainEvent>(organization);
        domainEvent.Organization.Should().Be(organization);
        domainEvent.User.Should().Be(user);
    }

    [Fact]
    public void RemoveUser_WhenUserHasProjects_ShouldReturnError()
    {
        // Arrange
        var organization = OrganizationFactory.CreateOrganization();
        var user = UserFactory.CreateUser();
        var project = ProjectFactory.CreateProject();
        organization.AddUser(user);
        organization.AddProject(project);
        project.SetOrganization(organization);
        project.Users.Add(user);
        project.AddUser(user);

        // Act
        var result = organization.RemoveUser(user.Id);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(OrganizationErrors.UserHasProjects);
        organization.Users.Should().HaveCount(1);
    }
}