using Application.Common.Interfaces.Persistence;
using Application.WikiPages.Commands.MoveWikiPage;
using Domain.Projects;
using Domain.WikiPages;

namespace Application.UnitTests.WikiPages.Commands;

public class MoveWikiPageTests
{
    private static readonly MoveWikiPageCommand Command = new(
        Guid.NewGuid(),
        Guid.NewGuid(),
        null,
        0);

    private readonly MoveWikiPageCommandHandler _handler;
    private readonly IProjectsRepository _projectsRepository;

    public MoveWikiPageTests()
    {
        _projectsRepository = Substitute.For<IProjectsRepository>();
        _handler = new MoveWikiPageCommandHandler(_projectsRepository);
    }

    [Fact]
    public async Task Handle_WhenProjectDoesNotExist_ShouldReturnError()
    {
        // Arrange
        _projectsRepository.GetByIdWithWikiPagesAsync(
            Command.ProjectId,
            TestContext.Current.CancellationToken)
            .Returns(null as Project);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.FirstError.Should().Be(ProjectErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenWikiPageDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var project = new Project("Project", "Description", Guid.NewGuid());
        _projectsRepository.GetByIdWithWikiPagesAsync(
            Command.ProjectId,
            TestContext.Current.CancellationToken)
            .Returns(project);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.FirstError.Should().Be(WikiPageErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenParentWikiPageDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var project = new Project("Project", "Description", Guid.NewGuid());
        var wikiPage = new WikiPage(project.Id, "WikiPage", "Content", Guid.NewGuid());
        project.AddWikiPage(wikiPage);
        _projectsRepository.GetByIdWithWikiPagesAsync(
            Command.ProjectId,
            TestContext.Current.CancellationToken)
            .Returns(project);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.FirstError.Should().Be(WikiPageErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenWikiExists_ShouldMoveWikiPage()
    {
        // Arrange
        var project = new Project("Project", "Description", Guid.NewGuid());
        var wikiPage1 = new WikiPage(project.Id, "WikiPage 1", "Content", Guid.NewGuid());
        var wikiPage2 = new WikiPage(project.Id, "WikiPage 2", "Content", Guid.NewGuid());
        project.AddWikiPage(wikiPage1);
        project.AddWikiPage(wikiPage2);
        _projectsRepository.GetByIdWithWikiPagesAsync(
            Command.ProjectId,
            TestContext.Current.CancellationToken)
            .Returns(project);

        // Act
        var result = await _handler.Handle(
            Command with { WikiPageId = wikiPage2.Id },
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        project.WikiPages.Should().HaveCount(2);
        wikiPage1.Position.Should().Be(1);
        wikiPage2.Position.Should().Be(0);
        _projectsRepository.Received().Update(project);
    }

    [Fact]
    public async Task Handle_WhenParentWikiPageExists_ShouldMoveWikiPage()
    {
        // Arrange
        var project = new Project("Project", "Description", Guid.NewGuid());
        var parentWikiPage = new WikiPage(project.Id, "Parent WikiPage", "Content", Guid.NewGuid());
        var wikiPage = new WikiPage(project.Id, "WikiPage", "Content", Guid.NewGuid());
        project.AddWikiPage(parentWikiPage);
        project.AddWikiPage(wikiPage);
        _projectsRepository.GetByIdWithWikiPagesAsync(
            Command.ProjectId,
            TestContext.Current.CancellationToken)
            .Returns(project);

        // Act
        var result = await _handler.Handle(
            Command with { WikiPageId = wikiPage.Id, TargetParentId = parentWikiPage.Id },
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        project.WikiPages.Should().HaveCount(1);
        project.WikiPages.First().Should().Be(parentWikiPage);
        parentWikiPage.SubWikiPages.Should().HaveCount(1);
        parentWikiPage.SubWikiPages.First().Should().Be(wikiPage);
        _projectsRepository.Received().Update(project);
    }

    [Fact]
    public async Task Handle_WhenPositionIsInvalid_ShouldReturnError()
    {
        // Arrange
        var project = new Project("Project", "Description", Guid.NewGuid());
        var wikiPage = new WikiPage(project.Id, "WikiPage", "Content", Guid.NewGuid());
        project.AddWikiPage(wikiPage);
        _projectsRepository.GetByIdWithWikiPagesAsync(
            Command.ProjectId,
            TestContext.Current.CancellationToken)
            .Returns(project);

        // Act
        var result = await _handler.Handle(
            Command with { WikiPageId = wikiPage.Id, TargetPosition = -1 },
            TestContext.Current.CancellationToken);

        // Assert
        result.FirstError.Should().Be(WikiPageErrors.InvalidPosition);
    }
}