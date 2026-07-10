using Domain.Authorization;
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
        var organization = OrganizationFactory.Create();

        // Assert
        var domainEvent = AssertDomainEventWasPublished<OrganizationCreatedDomainEvent>(organization);
        domainEvent.Organization.Should().Be(organization);
    }

    [Fact]
    public void CreateOrganization_ShouldSetProperties()
    {
        // Act
        var organization = OrganizationFactory.Create();

        // Assert
        organization.Name.Should().Be(OrganizationFactory.Name);
        organization.Description.Should().Be(OrganizationFactory.Description);
    }

    [Fact]
    public void UpdateOrganization_ShouldRaiseOrganizationUpdatedDomainEvent()
    {
        // Arrange
        var organization = OrganizationFactory.Create();

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
        var organization = OrganizationFactory.Create();
        var newName = "New Name";
        var newDescription = "New Description";

        // Act
        organization.Update(newName, newDescription);

        // Assert
        organization.Name.Should().Be(newName);
        organization.Description.Should().Be(newDescription);
    }

    [Fact]
    public void DeleteOrganization_ShouldRaiseOrganizationDeletedDomainEvent()
    {
        // Arrange
        var organization = OrganizationFactory.Create();

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
        var organization = OrganizationFactory.Create();
        var project = ProjectFactory.Create();
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
        var organization = OrganizationFactory.Create();
        var project = ProjectFactory.Create();

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
        var organization = OrganizationFactory.Create();
        var project = ProjectFactory.Create();

        // Act
        var result = organization.RemoveProject(project.Id);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.FirstError.Should().BeEquivalentTo(ProjectErrors.NotFound);
    }

    [Fact]
    public void RemoveProject_WhenProjectExists_ShouldRemoveProject()
    {
        // Arrange
        var organization = OrganizationFactory.Create();
        var project = ProjectFactory.Create();
        organization.AddProject(project);

        // Act
        var result = organization.RemoveProject(project.Id);

        // Assert
        result.IsError.Should().BeFalse();
        organization.Projects.Should().BeEmpty();
    }

    [Fact]
    public void AddMember_WhenUserDoesNotExist_ShouldAddMember()
    {
        // Arrange
        var organization = OrganizationFactory.Create();
        var user = UserFactory.Create();

        // Act
        var result = organization.AddMember(user, SystemRoles.OrganizationMember);

        // Assert
        result.IsError.Should().BeFalse();
        organization.Members.Should().HaveCount(1);
        organization.Members.First().UserId.Should().Be(user.Id);
    }

    [Fact]
    public void AddMember_WhenUserDoesNotExist_ShouldRaiseUserAddedToOrganizationDomainEvent()
    {
        // Arrange
        var organization = OrganizationFactory.Create();
        var user = UserFactory.Create();

        // Act
        organization.AddMember(user, SystemRoles.OrganizationMember);

        // Assert
        var domainEvent = AssertDomainEventWasPublished<UserAddedToOrganizationDomainEvent>(organization);
        domainEvent.Organization.Should().Be(organization);
        domainEvent.User.Should().Be(user);
    }

    [Fact]
    public void AddMember_WhenUserAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var organization = OrganizationFactory.Create();
        var user = UserFactory.Create();
        organization.AddMember(user, SystemRoles.OrganizationMember);

        // Act
        var result = organization.AddMember(user, SystemRoles.OrganizationMember);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(OrganizationErrors.UserAlreadyExists);
        organization.Members.Should().HaveCount(1);
    }

    [Fact]
    public void AddMember_WhenRoleScopeDoesNotMatch_ShouldReturnError()
    {
        // Arrange
        var organization = OrganizationFactory.Create();
        var user = UserFactory.Create();

        // Act
        var result = organization.AddMember(user, SystemRoles.ProjectAdmin);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(RoleErrors.ScopeMismatch);
    }

    [Fact]
    public void ChangeMemberRole_WhenMemberDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var organization = OrganizationFactory.Create();

        // Act
        var result = organization.ChangeMemberRole(Guid.NewGuid(), SystemRoles.OrganizationAdmin);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(UserErrors.NotFound);
    }

    [Fact]
    public void ChangeMemberRole_WhenMemberExists_ShouldChangeRole()
    {
        // Arrange
        var organization = OrganizationFactory.Create();
        var owner = UserFactory.Create();
        var member = UserFactory.Create(email: "member@example.com");
        organization.AddMember(owner, SystemRoles.OrganizationOwner);
        organization.AddMember(member, SystemRoles.OrganizationMember);

        // Act
        var result = organization.ChangeMemberRole(member.Id, SystemRoles.OrganizationAdmin);

        // Assert
        result.IsError.Should().BeFalse();
        organization.Members.Single(m => m.UserId == member.Id).RoleId.Should().Be(SystemRoles.OrganizationAdminId);
    }

    [Fact]
    public void ChangeMemberRole_WhenDemotingLastOwner_ShouldReturnError()
    {
        // Arrange
        var organization = OrganizationFactory.Create();
        var owner = UserFactory.Create();
        organization.AddMember(owner, SystemRoles.OrganizationOwner);

        // Act
        var result = organization.ChangeMemberRole(owner.Id, SystemRoles.OrganizationAdmin);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(OrganizationErrors.CannotRemoveLastOwner);
    }

    [Fact]
    public void RemoveUser_WhenUserDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var organization = OrganizationFactory.Create();
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
        var organization = OrganizationFactory.Create();
        var owner = UserFactory.Create();
        var user = UserFactory.Create(email: "member@example.com");
        organization.AddMember(owner, SystemRoles.OrganizationOwner);
        organization.AddMember(user, SystemRoles.OrganizationMember);

        // Act
        var result = organization.RemoveUser(user.Id);

        // Assert
        result.IsError.Should().BeFalse();
        organization.Members.Should().NotContain(m => m.UserId == user.Id);
    }

    [Fact]
    public void RemoveUser_WhenUserRemoved_ShouldRaiseUserRemovedFromOrganizationDomainEvent()
    {
        // Arrange
        var organization = OrganizationFactory.Create();
        var owner = UserFactory.Create();
        var user = UserFactory.Create(email: "member@example.com");
        organization.AddMember(owner, SystemRoles.OrganizationOwner);
        organization.AddMember(user, SystemRoles.OrganizationMember);

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
        var organization = OrganizationFactory.Create();
        var user = UserFactory.Create();
        var project = ProjectFactory.Create();
        organization.AddMember(user, SystemRoles.OrganizationOwner);
        organization.AddProject(project);
        project.SetOrganization(organization);
        project.AddMember(user, SystemRoles.ProjectAdmin);

        // Act
        var result = organization.RemoveUser(user.Id);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(OrganizationErrors.UserHasProjects);
        organization.Members.Should().HaveCount(1);
    }

    [Fact]
    public void RemoveUser_WhenRemovingLastOwner_ShouldReturnError()
    {
        // Arrange
        var organization = OrganizationFactory.Create();
        var owner = UserFactory.Create();
        organization.AddMember(owner, SystemRoles.OrganizationOwner);

        // Act
        var result = organization.RemoveUser(owner.Id);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(OrganizationErrors.CannotRemoveLastOwner);
    }
}
