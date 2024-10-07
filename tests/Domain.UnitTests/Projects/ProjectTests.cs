using Domain.Projects;
using Domain.Tags;
using TestCommon.Projects;
using TestCommon.Tags;
using TestCommon.Teams;
using TestCommon.WikiPages;
using TestCommon.WorkItems;

namespace Domain.UnitTests.Projects;

public class ProjectTests
{
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
        result.Errors.First().Should().BeEquivalentTo(ProjectErrors.WorkItemWithSameTitleAlreadyExists);
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
        result.Errors.First().Should().BeEquivalentTo(ProjectErrors.WikiPageWithSameTitleAlreadyExists);
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
        result.Errors.First().Should().BeEquivalentTo(ProjectErrors.TeamWithSameNameAlreadyExists);
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
        result.Errors.First().Should().BeEquivalentTo(TagErrors.AlreadyExists);
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
    public void RemoveTag_WhenTagDoesNotExists_ShouldReturnError()
    {
        // Arrange
        var project = ProjectFactory.CreateProject();
        var tag = TagFactory.CreateTag();

        // Act
        var result = project.RemoveTag(tag.Name);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Should().BeEquivalentTo(TagErrors.NotFound);
    }

    [Fact]
    public void RemoveTag_WhenTagExists_ShouldRemoveTag()
    {
        // Arrange
        var project = ProjectFactory.CreateProject();
        var tag = TagFactory.CreateTag();
        project.AddTag(tag);

        // Act
        var result = project.RemoveTag(tag.Name);

        // Assert
        result.IsError.Should().BeFalse();
        project.Tags.Should().BeEmpty();
    }
}