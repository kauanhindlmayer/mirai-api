using Mirai.Domain.Projects;
using TestCommon.Projects;
using TestCommon.WikiPages;
using TestCommon.WorkItems;

namespace Mirai.Domain.UnitTests.Projects;

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
}