using Domain.Personas;
using Domain.Projects;
using Domain.Projects.Events;
using Domain.Tags;
using Domain.Teams;
using Domain.UnitTests.Boards;
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
        var project = ProjectFactory.CreateProject();

        // Assert
        project.Name.Should().Be(ProjectFactory.Name);
        project.Description.Should().Be(ProjectFactory.Description);
        project.OrganizationId.Should().Be(ProjectFactory.OrganizationId);
    }

    [Fact]
    public void CreateProject_ShouldRaiseProjectCreatedDomainEvent()
    {
        // Act
        var project = ProjectFactory.CreateProject();

        // Assert
        var domainEvent = AssertDomainEventWasPublished<ProjectCreatedDomainEvent>(project);
        domainEvent.Project.Should().Be(project);
    }

    [Fact]
    public void Update_ShouldUpdateProperties()
    {
        // Arrange
        var project = ProjectFactory.CreateProject();
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
        var project = ProjectFactory.CreateProject();
        var wikiPage = WikiPageFactory.CreateWikiPage();
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
        var project = ProjectFactory.CreateProject();
        var wikiPage = WikiPageFactory.CreateWikiPage();

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
        var project = ProjectFactory.CreateProject();
        var wikiPage = WikiPageFactory.CreateWikiPage();

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
        var project = ProjectFactory.CreateProject();
        var wikiPage = WikiPageFactory.CreateWikiPage();
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
        var project = ProjectFactory.CreateProject();
        var wikiPage = WikiPageFactory.CreateWikiPage(project.Id);
        project.AddWikiPage(wikiPage);
        var targetParent = WikiPageFactory.CreateWikiPage(project.Id, "Wiki Page 2");
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
        var project = ProjectFactory.CreateProject();
        var wikiPage1 = WikiPageFactory.CreateWikiPage(project.Id, "Wiki Page 1");
        var wikiPage2 = WikiPageFactory.CreateWikiPage(project.Id, "Wiki Page 2");
        var wikiPage3 = WikiPageFactory.CreateWikiPage(project.Id, "Wiki Page 3");
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
        var project = ProjectFactory.CreateProject();
        var team = TeamFactory.CreateTeam();
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
        var project = ProjectFactory.CreateProject();
        var team = TeamFactory.CreateTeam();

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
        var project = ProjectFactory.CreateProject();
        var tag = TagFactory.CreateTag();
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
        var project = ProjectFactory.CreateProject();
        var tag = TagFactory.CreateTag();

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
        var project = ProjectFactory.CreateProject();
        var tag = TagFactory.CreateTag();
        project.AddTag(tag);

        // Act
        var result = project.RemoveTag(tag.Id);

        // Assert
        result.IsError.Should().BeFalse();
        project.Tags.Should().BeEmpty();
    }

    [Fact]
    public void AddUser_WhenUserNotInOrganization_ShouldReturnError()
    {
        // Arrange
        var project = ProjectFactory.CreateProject();
        var user = UserFactory.CreateUser();
        var organization = OrganizationFactory.CreateOrganization();
        project.SetOrganization(organization);

        // Act
        var result = project.AddUser(user);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(ProjectErrors.UserNotInOrganization);
        project.Users.Should().BeEmpty();
    }

    [Fact]
    public void AddUser_WhenUserAlreadyInProject_ShouldReturnError()
    {
        // Arrange
        var project = ProjectFactory.CreateProject();
        var user = UserFactory.CreateUser();
        var organization = OrganizationFactory.CreateOrganization();
        project.SetOrganization(organization);
        project.Organization.AddUser(user);
        project.AddUser(user);

        // Act
        var result = project.AddUser(user);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(ProjectErrors.UserAlreadyExists);
        project.Users.Should().HaveCount(1);
    }

    [Fact]
    public void AddUser_WhenUserInOrganizationAndNotInProject_ShouldAddUser()
    {
        // Arrange
        var project = ProjectFactory.CreateProject();
        var user = UserFactory.CreateUser();
        var organization = OrganizationFactory.CreateOrganization();
        project.SetOrganization(organization);
        project.Organization.AddUser(user);

        // Act
        var result = project.AddUser(user);

        // Assert
        result.IsError.Should().BeFalse();
        project.Users.Should().HaveCount(1);
        project.Users.Should().Contain(user);
    }

    [Fact]
    public void AddUser_WhenUserAdded_ShouldRaiseUserAddedToProjectDomainEvent()
    {
        // Arrange
        var project = ProjectFactory.CreateProject();
        var user = UserFactory.CreateUser();
        var organization = OrganizationFactory.CreateOrganization();
        project.SetOrganization(organization);
        project.Organization.AddUser(user);
        project.ClearDomainEvents();

        // Act
        var result = project.AddUser(user);

        // Assert
        result.IsError.Should().BeFalse();
        var domainEvent = AssertDomainEventWasPublished<UserAddedToProjectDomainEvent>(project);
        domainEvent.Project.Should().Be(project);
        domainEvent.User.Should().Be(user);
    }

    [Fact]
    public void RemoveUser_WhenUserNotInProject_ShouldReturnError()
    {
        // Arrange
        var project = ProjectFactory.CreateProject();
        var userId = Guid.NewGuid();
        var organization = OrganizationFactory.CreateOrganization();
        project.SetOrganization(organization);

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
        var project = ProjectFactory.CreateProject();
        var user = UserFactory.CreateUser();
        var team = TeamFactory.CreateTeam(project.Id);
        var organization = OrganizationFactory.CreateOrganization();
        project.SetOrganization(organization);
        project.Organization.AddUser(user);
        project.AddUser(user);
        project.AddTeam(team);
        team.AddMember(user);

        // Act
        var result = project.RemoveUser(user.Id);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(ProjectErrors.UserIsInTeams);
        project.Users.Should().Contain(user);
    }

    [Fact]
    public void RemoveUser_WhenUserCanBeRemoved_ShouldRemoveUser()
    {
        // Arrange
        var project = ProjectFactory.CreateProject();
        var user = UserFactory.CreateUser();
        var organization = OrganizationFactory.CreateOrganization();
        project.SetOrganization(organization);
        project.Organization.AddUser(user);
        project.AddUser(user);

        // Act
        var result = project.RemoveUser(user.Id);

        // Assert
        result.IsError.Should().BeFalse();
        project.Users.Should().NotContain(user);
    }

    [Fact]
    public void RemoveUser_WhenUserRemoved_ShouldRaiseUserRemovedFromProjectDomainEvent()
    {
        // Arrange
        var project = ProjectFactory.CreateProject();
        var user = UserFactory.CreateUser();
        var organization = OrganizationFactory.CreateOrganization();
        project.SetOrganization(organization);
        project.Organization.AddUser(user);
        project.AddUser(user);
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
    public void AddWorkItem_WhenTeamNotFound_ShouldReturnError()
    {
        // Arrange
        var project = ProjectFactory.CreateProject();
        var workItem = WorkItemFactory.CreateWorkItem(assignedTeamId: Guid.NewGuid());

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
        var project = ProjectFactory.CreateProject();
        var parentId = Guid.NewGuid();
        var wikiPage = WikiPageFactory.CreateWikiPage(project.Id, parentWikiPageId: parentId);

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
        var project = ProjectFactory.CreateProject();
        var parentWikiPage = WikiPageFactory.CreateWikiPage(project.Id);
        project.AddWikiPage(parentWikiPage);
        var childWikiPage = WikiPageFactory.CreateWikiPage(
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
        var project = ProjectFactory.CreateProject();
        var wikiPage = WikiPageFactory.CreateWikiPage(project.Id);
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
        var project = ProjectFactory.CreateProject();
        var persona = PersonaFactory.CreatePersona(project.Id);
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
        var project = ProjectFactory.CreateProject();
        var persona = PersonaFactory.CreatePersona(project.Id);

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
        var project = ProjectFactory.CreateProject();
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
        var project = ProjectFactory.CreateProject();
        var persona = PersonaFactory.CreatePersona(project.Id);
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
        var project = ProjectFactory.CreateProject();
        var newOrganization = OrganizationFactory.CreateOrganization();

        // Act
        project.SetOrganization(newOrganization);

        // Assert
        project.Organization.Should().Be(newOrganization);
        project.OrganizationId.Should().Be(newOrganization.Id);
    }
}