using Domain.Projects;
using Domain.Projects.Events;
using Domain.Tags;
using Domain.UnitTests.Infrastructure;
using Domain.UnitTests.Tags;
using Domain.UnitTests.Teams;
using Domain.UnitTests.WikiPages;
using Domain.UnitTests.WorkItems;
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
    public void AddWorkItem_WhenWorkItemWithSameTitleAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var project = ProjectFactory.CreateProject();
        var workItem = WorkItemFactory.CreateWorkItem();
        project.AddWorkItem(workItem);

        // Act
        var result = project.AddWorkItem(workItem);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.FirstError.Should().BeEquivalentTo(ProjectErrors.WorkItemWithSameTitleAlreadyExists);
    }

    [Fact]
    public void AddWorkItem_WhenWorkItemWithSameTitleDoesNotExists_ShouldAddWorkItem()
    {
        // Arrange
        var project = ProjectFactory.CreateProject();
        var workItem = WorkItemFactory.CreateWorkItem();

        // Act
        var result = project.AddWorkItem(workItem);

        // Assert
        result.IsError.Should().BeFalse();
        project.WorkItems.Should().HaveCount(1);
        project.WorkItems.First().Should().BeEquivalentTo(workItem);
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
        var wikiPage = WikiPageFactory.CreateWikiPage(project.Id);
        project.AddWikiPage(wikiPage);
        var wikiPage2 = WikiPageFactory.CreateWikiPage(project.Id, "Wiki Page 2");
        project.AddWikiPage(wikiPage2);

        // Act
        var result = project.MoveWikiPage(wikiPage2.Id, null, 0);

        // Assert
        result.IsError.Should().BeFalse();
        project.WikiPages.Should().HaveCount(2);
        project.WikiPages.First().Should().BeEquivalentTo(wikiPage2);
        project.WikiPages.Last().Should().BeEquivalentTo(wikiPage);
    }

    [Fact]
    public void ReorderWikiPages_ShouldUpdatePositions()
    {
        // Arrange
        var project = ProjectFactory.CreateProject();
        var wikiPage = WikiPageFactory.CreateWikiPage(project.Id);
        project.AddWikiPage(wikiPage);
        var wikiPage2 = WikiPageFactory.CreateWikiPage(project.Id, "Wiki Page 2");
        project.AddWikiPage(wikiPage2);

        // Act
        project.ReorderWikiPages();

        // Assert
        project.WikiPages.First().Position.Should().Be(0);
        project.WikiPages.Last().Position.Should().Be(1);
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
        var result = project.RemoveTag(tag);

        // Assert
        result.IsError.Should().BeFalse();
        project.Tags.Should().BeEmpty();
    }
}