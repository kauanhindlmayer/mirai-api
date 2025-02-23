using Domain.Organizations;
using Domain.Organizations.Events;
using Domain.Projects;
using Domain.UnitTests.Common;
using Domain.UnitTests.Projects;
using Domain.UnitTests.Users;

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
    public void AddMember_WhenMemberAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var organization = OrganizationFactory.CreateOrganization();
        var member = UserFactory.CreateUser();
        organization.AddMember(member);

        // Act
        var result = organization.AddMember(member);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.FirstError.Should().BeEquivalentTo(OrganizationErrors.UserAlreadyMember);
    }

    [Fact]
    public void AddMember_WhenMemberDoesNotExists_ShouldAddMember()
    {
        // Arrange
        var organization = OrganizationFactory.CreateOrganization();
        var member = UserFactory.CreateUser();

        // Act
        var result = organization.AddMember(member);

        // Assert
        result.IsError.Should().BeFalse();
        organization.Members.Should().HaveCount(1);
        organization.Members.First().Should().BeEquivalentTo(member);
    }

    [Fact]
    public void RemoveMember_WhenMemberDoesNotExists_ShouldReturnError()
    {
        // Arrange
        var organization = OrganizationFactory.CreateOrganization();
        var member = UserFactory.CreateUser();

        // Act
        var result = organization.RemoveMember(member);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.FirstError.Should().BeEquivalentTo(OrganizationErrors.UserNotMember);
    }

    [Fact]
    public void RemoveMember_WhenMemberExists_ShouldRemoveMember()
    {
        // Arrange
        var organization = OrganizationFactory.CreateOrganization();
        var member = UserFactory.CreateUser();
        organization.AddMember(member);

        // Act
        var result = organization.RemoveMember(member);

        // Assert
        result.IsError.Should().BeFalse();
        organization.Members.Should().BeEmpty();
    }

    [Fact]
    public void RemoveMember_WhenMemberExists_ShouldRemoveCorrectMember()
    {
        // Arrange
        var organization = OrganizationFactory.CreateOrganization();
        var member1 = UserFactory.CreateUser();
        var member2 = UserFactory.CreateUser();
        organization.AddMember(member1);
        organization.AddMember(member2);

        // Act
        var result = organization.RemoveMember(member1);

        // Assert
        result.IsError.Should().BeFalse();
        organization.Members.Should().HaveCount(1);
        organization.Members.First().Should().BeEquivalentTo(member2);
    }
}