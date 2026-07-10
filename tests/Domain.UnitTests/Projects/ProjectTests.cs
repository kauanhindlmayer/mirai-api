using Domain.Authorization;
using Domain.Personas;
using Domain.Projects;
using Domain.Projects.Events;
using Domain.Tags;
using Domain.Teams;
using Domain.UnitTests.Infrastructure;
using Domain.UnitTests.Organizations;
using Domain.UnitTests.Personas;
using Domain.UnitTests.Tags;
using Domain.UnitTests.Teams;
using Domain.UnitTests.Users;
using Domain.UnitTests.WikiPages;
using Domain.UnitTests.WorkItems;
using Domain.Users;
using Domain.WikiPages;

namespace Domain.UnitTests.Projects;

public class ProjectTests : BaseTest
{
    [Fact]
    public void CreateProject_ShouldSetProperties()
    {
        // Act
        var project = ProjectFactory.Create();

        // Assert
        project.Name.Should().Be(ProjectFactory.Name);
        project.Description.Should().Be(ProjectFactory.Description);
        project.OrganizationId.Should().Be(ProjectFactory.OrganizationId);
    }

    [Fact]
    public void CreateProject_ShouldRaiseProjectCreatedDomainEvent()
    {
        // Act
        var project = ProjectFactory.Create();

        // Assert
        var domainEvent = AssertDomainEventWasPublished<ProjectCreatedDomainEvent>(project);
        domainEvent.Project.Should().Be(project);
    }

    [Fact]
    public void Update_ShouldUpdateProperties()
    {
        // Arrange
        var project = ProjectFactory.Create();
        var name = "New Name";
        var description = "New Description";

        // Act
        project.Update(name, description);

        // Assert
        project.Name.Should().Be(name);
        project.Description.Should().Be(description);
    }

    [Fact]
    public void AddWikiPage_WhenWikiPageWithSameTitleAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var project = ProjectFactory.Create();
        var wikiPage = WikiPageFactory.Create();
        project.AddWikiPage(wikiPage);

        // Act
        var result = project.AddWikiPage(wikiPage);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.FirstError.Should().BeEquivalentTo(ProjectErrors.WikiPageWithSameTitleAlreadyExists);
    }

    [Fact]
    public void AddWikiPage_WhenWikiPageWithSameTitleDoesNotExists_ShouldAddWikiPage()
    {
        // Arrange
        var project = ProjectFactory.Create();
        var wikiPage = WikiPageFactory.Create();

        // Act
        var result = project.AddWikiPage(wikiPage);

        // Assert
        result.IsError.Should().BeFalse();
        project.WikiPages.Should().HaveCount(1);
        project.WikiPages.First().Should().BeEquivalentTo(wikiPage);
    }

    [Fact]
    public void MoveWikiPage_WhenWikiPageDoesNotExists_ShouldReturnError()
    {
        // Arrange
        var project = ProjectFactory.Create();
        var wikiPage = WikiPageFactory.Create();

        // Act
        var result = project.MoveWikiPage(wikiPage.Id, null, 0);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.FirstError.Should().BeEquivalentTo(WikiPageErrors.NotFound);
    }

    [Fact]
    public void MoveWikiPage_WhenTargetParentDoesNotExists_ShouldReturnError()
    {
        // Arrange
        var project = ProjectFactory.Create();
        var wikiPage = WikiPageFactory.Create(project.Id);
        project.AddWikiPage(wikiPage);

        // Act
        var result = project.MoveWikiPage(wikiPage.Id, Guid.NewGuid(), 0);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.FirstError.Should().BeEquivalentTo(WikiPageErrors.ParentWikiPageNotFound);
    }

    [Fact]
    public void MoveWikiPage_WhenTargetParentExists_ShouldMoveWikiPage()
    {
        // Arrange
        var project = ProjectFactory.Create();
        var wikiPage = WikiPageFactory.Create(project.Id, "Wiki Page 1");
        project.AddWikiPage(wikiPage);
        var targetParent = WikiPageFactory.Create(project.Id, "Wiki Page 2");
        project.AddWikiPage(targetParent);

        // Act
        var result = project.MoveWikiPage(wikiPage.Id, targetParent.Id, 0);

        // Assert
        result.IsError.Should().BeFalse();
        project.WikiPages.Should().HaveCount(1);
        project.WikiPages.First().Should().BeEquivalentTo(targetParent);
    }

    [Fact]
    public void MoveWikiPage_WhenWikiPageExists_ShouldMoveWikiPage()
    {
        // Arrange
        var project = ProjectFactory.Create();
        var wikiPage1 = WikiPageFactory.Create(project.Id, "Wiki Page 1");
        var wikiPage2 = WikiPageFactory.Create(project.Id, "Wiki Page 2");
        var wikiPage3 = WikiPageFactory.Create(project.Id, "Wiki Page 3");
        project.AddWikiPage(wikiPage1);
        project.AddWikiPage(wikiPage2);
        project.AddWikiPage(wikiPage3);

        // Act
        var result = project.MoveWikiPage(wikiPage3.Id, null, 1);

        // Assert
        result.IsError.Should().BeFalse();
        project.WikiPages.Should().HaveCount(3);
        wikiPage1.Position.Should().Be(0);
        wikiPage3.Position.Should().Be(1);
        wikiPage2.Position.Should().Be(2);
    }

    [Fact]
    public void AddTeam_WhenTeamWithSameNameAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var project = ProjectFactory.Create();
        var team = TeamFactory.Create();
        project.AddTeam(team);

        // Act
        var result = project.AddTeam(team);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.FirstError.Should().BeEquivalentTo(ProjectErrors.TeamWithSameNameAlreadyExists);
    }

    [Fact]
    public void AddTeam_WhenTeamWithSameNameDoesNotExists_ShouldAddTeam()
    {
        // Arrange
        var project = ProjectFactory.Create();
        var team = TeamFactory.Create();

        // Act
        var result = project.AddTeam(team);

        // Assert
        result.IsError.Should().BeFalse();
        project.Teams.Should().HaveCount(1);
        project.Teams.First().Should().BeEquivalentTo(team);
    }

    [Fact]
    public void AddTag_WhenTagWithSameNameAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var project = ProjectFactory.Create();
        var tag = TagFactory.Create();
        project.AddTag(tag);

        // Act
        var result = project.AddTag(tag);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.FirstError.Should().BeEquivalentTo(TagErrors.AlreadyExists);
    }

    [Fact]
    public void AddTag_WhenTagWithSameNameDoesNotExists_ShouldAddTag()
    {
        // Arrange
        var project = ProjectFactory.Create();
        var tag = TagFactory.Create();

        // Act
        var result = project.AddTag(tag);

        // Assert
        result.IsError.Should().BeFalse();
        project.Tags.Should().HaveCount(1);
        project.Tags.First().Should().BeEquivalentTo(tag);
    }

    [Fact]
    public void RemoveTag_WhenTagExists_ShouldRemoveTag()
    {
        // Arrange
        var project = ProjectFactory.Create();
        var tag = TagFactory.Create();
        project.AddTag(tag);

        // Act
        var result = project.RemoveTag(tag.Id);

        // Assert
        result.IsError.Should().BeFalse();
        project.Tags.Should().BeEmpty();
    }

    [Fact]
    public void AddMember_WhenRoleScopeDoesNotMatch_ShouldReturnError()
    {
        // Arrange
        var project = ProjectFactory.Create();
        var user = UserFactory.Create();

        // Act
        var result = project.AddMember(user, SystemRoles.OrganizationMember);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(RoleErrors.ScopeMismatch);
        project.Members.Should().BeEmpty();
    }

    [Fact]
    public void AddMember_WhenUserAlreadyInProject_ShouldReturnError()
    {
        // Arrange
        var project = ProjectFactory.Create();
        var user = UserFactory.Create();
        project.AddMember(user, SystemRoles.ProjectAdmin);

        // Act
        var result = project.AddMember(user, SystemRoles.ProjectContributor);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(ProjectErrors.UserAlreadyExists);
        project.Members.Should().HaveCount(1);
    }

    [Fact]
    public void AddMember_WhenUserNotInProject_ShouldAddMember()
    {
        // Arrange
        var project = ProjectFactory.Create();
        var user = UserFactory.Create();

        // Act
        var result = project.AddMember(user, SystemRoles.ProjectContributor);

        // Assert
        result.IsError.Should().BeFalse();
        project.Members.Should().HaveCount(1);
        project.Members.First().UserId.Should().Be(user.Id);
    }

    [Fact]
    public void AddMember_WhenUserAdded_ShouldRaiseUserAddedToProjectDomainEvent()
    {
        // Arrange
        var project = ProjectFactory.Create();
        var user = UserFactory.Create();
        project.ClearDomainEvents();

        // Act
        var result = project.AddMember(user, SystemRoles.ProjectContributor);

        // Assert
        result.IsError.Should().BeFalse();
        var domainEvent = AssertDomainEventWasPublished<UserAddedToProjectDomainEvent>(project);
        domainEvent.Project.Should().Be(project);
        domainEvent.User.Should().Be(user);
    }

    [Fact]
    public void ChangeMemberRole_WhenDemotingLastAdmin_ShouldReturnError()
    {
        // Arrange
        var project = ProjectFactory.Create();
        var admin = UserFactory.Create();
        project.AddMember(admin, SystemRoles.ProjectAdmin);

        // Act
        var result = project.ChangeMemberRole(admin.Id, SystemRoles.ProjectContributor);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(ProjectErrors.CannotRemoveLastAdmin);
    }

    [Fact]
    public void RemoveUser_WhenUserNotInProject_ShouldReturnError()
    {
        // Arrange
        var project = ProjectFactory.Create();
        var userId = Guid.NewGuid();

        // Act
        var result = project.RemoveUser(userId);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(UserErrors.NotFound);
    }

    [Fact]
    public void RemoveUser_WhenUserIsInTeams_ShouldReturnError()
    {
        // Arrange
        var project = ProjectFactory.Create();
        var admin = UserFactory.Create();
        var user = UserFactory.Create(email: "member@example.com");
        var team = TeamFactory.Create(project.Id);
        project.AddMember(admin, SystemRoles.ProjectAdmin);
        project.AddMember(user, SystemRoles.ProjectContributor);
        project.AddTeam(team);
        team.AddMember(user, SystemRoles.TeamMember);

        // Act
        var result = project.RemoveUser(user.Id);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(ProjectErrors.UserIsInTeams);
        project.Members.Should().Contain(m => m.UserId == user.Id);
    }

    [Fact]
    public void RemoveUser_WhenUserCanBeRemoved_ShouldRemoveUser()
    {
        // Arrange
        var project = ProjectFactory.Create();
        var admin = UserFactory.Create();
        var user = UserFactory.Create(email: "member@example.com");
        project.AddMember(admin, SystemRoles.ProjectAdmin);
        project.AddMember(user, SystemRoles.ProjectContributor);

        // Act
        var result = project.RemoveUser(user.Id);

        // Assert
        result.IsError.Should().BeFalse();
        project.Members.Should().NotContain(m => m.UserId == user.Id);
    }

    [Fact]
    public void RemoveUser_WhenUserRemoved_ShouldRaiseUserRemovedFromProjectDomainEvent()
    {
        // Arrange
        var project = ProjectFactory.Create();
        var admin = UserFactory.Create();
        var user = UserFactory.Create(email: "member@example.com");
        project.AddMember(admin, SystemRoles.ProjectAdmin);
        project.AddMember(user, SystemRoles.ProjectContributor);
        project.ClearDomainEvents();

        // Act
        var result = project.RemoveUser(user.Id);

        // Assert
        result.IsError.Should().BeFalse();
        var domainEvent = AssertDomainEventWasPublished<UserRemovedFromProjectDomainEvent>(project);
        domainEvent.Project.Should().Be(project);
        domainEvent.User.Should().Be(user);
    }

    [Fact]
    public void RemoveUser_WhenRemovingLastAdmin_ShouldReturnError()
    {
        // Arrange
        var project = ProjectFactory.Create();
        var admin = UserFactory.Create();
        project.AddMember(admin, SystemRoles.ProjectAdmin);

        // Act
        var result = project.RemoveUser(admin.Id);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(ProjectErrors.CannotRemoveLastAdmin);
    }

    [Fact]
    public void AddWorkItem_WhenTeamNotFound_ShouldReturnError()
    {
        // Arrange
        var project = ProjectFactory.Create();
        var workItem = WorkItemFactory.Create(assignedTeamId: Guid.NewGuid());

        // Act
        var result = project.AddWorkItem(workItem);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(TeamErrors.NotFound);
        project.WorkItems.Should().BeEmpty();
    }

    [Fact]
    public void AddWikiPage_WhenParentWikiPageNotFound_ShouldReturnError()
    {
        // Arrange
        var project = ProjectFactory.Create();
        var parentId = Guid.NewGuid();
        var wikiPage = WikiPageFactory.Create(project.Id, parentWikiPageId: parentId);

        // Act
        var result = project.AddWikiPage(wikiPage);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(WikiPageErrors.ParentWikiPageNotFound);
        project.WikiPages.Should().BeEmpty();
    }

    [Fact]
    public void AddWikiPage_WhenParentExists_ShouldAddAsSubWikiPage()
    {
        // Arrange
        var project = ProjectFactory.Create();
        var parentWikiPage = WikiPageFactory.Create(project.Id);
        project.AddWikiPage(parentWikiPage);
        var childWikiPage = WikiPageFactory.Create(
            project.Id,
            "Child Page",
            parentWikiPageId: parentWikiPage.Id);

        // Act
        var result = project.AddWikiPage(childWikiPage);

        // Assert
        result.IsError.Should().BeFalse();
        project.WikiPages.Should().HaveCount(1);
        parentWikiPage.SubWikiPages.Should().Contain(childWikiPage);
    }

    [Fact]
    public void MoveWikiPage_WhenTargetPositionInvalid_ShouldReturnError()
    {
        // Arrange
        var project = ProjectFactory.Create();
        var wikiPage = WikiPageFactory.Create(project.Id);
        project.AddWikiPage(wikiPage);

        // Act
        var result = project.MoveWikiPage(wikiPage.Id, null, -1);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(WikiPageErrors.InvalidPosition);
    }

    [Fact]
    public void AddPersona_WhenPersonaWithSameNameAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var project = ProjectFactory.Create();
        var persona = PersonaFactory.Create(project.Id);
        project.AddPersona(persona);

        // Act
        var result = project.AddPersona(persona);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(PersonaErrors.AlreadyExists);
        project.Personas.Should().HaveCount(1);
    }

    [Fact]
    public void AddPersona_WhenPersonaWithSameNameDoesNotExist_ShouldAddPersona()
    {
        // Arrange
        var project = ProjectFactory.Create();
        var persona = PersonaFactory.Create(project.Id);

        // Act
        var result = project.AddPersona(persona);

        // Assert
        result.IsError.Should().BeFalse();
        project.Personas.Should().HaveCount(1);
        project.Personas.Should().Contain(persona);
    }

    [Fact]
    public void RemovePersona_WhenPersonaNotFound_ShouldReturnError()
    {
        // Arrange
        var project = ProjectFactory.Create();
        var personaId = Guid.NewGuid();

        // Act
        var result = project.RemovePersona(personaId);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(PersonaErrors.NotFound);
    }

    [Fact]
    public void RemovePersona_WhenPersonaExists_ShouldRemovePersona()
    {
        // Arrange
        var project = ProjectFactory.Create();
        var persona = PersonaFactory.Create(project.Id);
        project.AddPersona(persona);

        // Act
        var result = project.RemovePersona(persona.Id);

        // Assert
        result.IsError.Should().BeFalse();
        project.Personas.Should().BeEmpty();
    }

    [Fact]
    public void SetOrganization_ShouldUpdateOrganizationAndId()
    {
        // Arrange
        var project = ProjectFactory.Create();
        var newOrganization = OrganizationFactory.Create();

        // Act
        project.SetOrganization(newOrganization);

        // Assert
        project.Organization.Should().Be(newOrganization);
        project.OrganizationId.Should().Be(newOrganization.Id);
    }

    [Fact]
    public void ConnectGitHubRepository_WhenNotConnected_ShouldConnect()
    {
        // Arrange
        var project = ProjectFactory.Create();

        // Act
        var result = project.ConnectGitHubRepository(
            GitHubRepositoryConnectionFactory.InstallationId,
            GitHubRepositoryConnectionFactory.RepositoryId,
            GitHubRepositoryConnectionFactory.RepositoryOwner,
            GitHubRepositoryConnectionFactory.RepositoryName,
            GitHubRepositoryConnectionFactory.ConnectedByUserId);

        // Assert
        result.IsError.Should().BeFalse();
        project.GitHubRepositoryConnection.Should().NotBeNull();
        project.GitHubRepositoryConnection!.RepositoryId.Should().Be(GitHubRepositoryConnectionFactory.RepositoryId);
        project.GitHubRepositoryConnection!.RepositoryOwner.Should().Be(GitHubRepositoryConnectionFactory.RepositoryOwner);
        project.GitHubRepositoryConnection!.RepositoryName.Should().Be(GitHubRepositoryConnectionFactory.RepositoryName);
    }

    [Fact]
    public void ConnectGitHubRepository_WhenAlreadyConnected_ShouldReturnError()
    {
        // Arrange
        var project = ProjectFactory.Create();
        project.ConnectGitHubRepository(
            GitHubRepositoryConnectionFactory.InstallationId,
            GitHubRepositoryConnectionFactory.RepositoryId,
            GitHubRepositoryConnectionFactory.RepositoryOwner,
            GitHubRepositoryConnectionFactory.RepositoryName,
            GitHubRepositoryConnectionFactory.ConnectedByUserId);

        // Act
        var result = project.ConnectGitHubRepository(
            GitHubRepositoryConnectionFactory.InstallationId,
            999,
            "other-owner",
            "other-repo",
            GitHubRepositoryConnectionFactory.ConnectedByUserId);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(ProjectErrors.GitHubRepositoryAlreadyConnected);
        project.GitHubRepositoryConnection!.RepositoryId.Should().Be(GitHubRepositoryConnectionFactory.RepositoryId);
    }

    [Fact]
    public void DisconnectGitHubRepository_WhenConnected_ShouldDisconnect()
    {
        // Arrange
        var project = ProjectFactory.Create();
        project.ConnectGitHubRepository(
            GitHubRepositoryConnectionFactory.InstallationId,
            GitHubRepositoryConnectionFactory.RepositoryId,
            GitHubRepositoryConnectionFactory.RepositoryOwner,
            GitHubRepositoryConnectionFactory.RepositoryName,
            GitHubRepositoryConnectionFactory.ConnectedByUserId);

        // Act
        var result = project.DisconnectGitHubRepository();

        // Assert
        result.IsError.Should().BeFalse();
        project.GitHubRepositoryConnection.Should().BeNull();
    }

    [Fact]
    public void DisconnectGitHubRepository_WhenNotConnected_ShouldReturnError()
    {
        // Arrange
        var project = ProjectFactory.Create();

        // Act
        var result = project.DisconnectGitHubRepository();

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(ProjectErrors.NoGitHubRepositoryConnected);
    }
}