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
    private readonly IProjectRepository _projectRepository;

    public MoveWikiPageTests()
    {
        _projectRepository = Substitute.For<IProjectRepository>();
        _handler = new MoveWikiPageCommandHandler(_projectRepository);
    }

    [Fact]
    public async Task Handle_WhenProjectDoesNotExist_ShouldReturnError()
    {
        // Arrange
        _projectRepository.GetByIdWithWikiPagesAsync(
            Command.ProjectId,
            TestContext.Current.CancellationToken)
            .Returns(null as Project);

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

        // Assert
        result.FirstError.Should().Be(ProjectErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenWikiPageDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var project = new Project("Project", "Description", Guid.NewGuid());
        _projectRepository.GetByIdWithWikiPagesAsync(
            Command.ProjectId,
            TestContext.Current.CancellationToken)
            .Returns(project);

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

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
        _projectRepository.GetByIdWithWikiPagesAsync(
            Command.ProjectId,
            TestContext.Current.CancellationToken)
            .Returns(project);

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

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
        _projectRepository.GetByIdWithWikiPagesAsync(
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
        _projectRepository.Received().Update(project);
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
        _projectRepository.GetByIdWithWikiPagesAsync(
            Command.ProjectId,
            TestContext.Current.CancellationToken)
            .Returns(project);

        // Act
        var result = await _handler.Handle(
            Command with
            {
                WikiPageId = wikiPage.Id,
                TargetParentId = parentWikiPage.Id,
            },
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        project.WikiPages.Should().HaveCount(1);
        project.WikiPages.First().Should().Be(parentWikiPage);
        parentWikiPage.SubWikiPages.Should().HaveCount(1);
        parentWikiPage.SubWikiPages.First().Should().Be(wikiPage);
        _projectRepository.Received().Update(project);
    }

    [Fact]
    public async Task Handle_WhenPositionIsInvalid_ShouldReturnError()
    {
        // Arrange
        var project = new Project("Project", "Description", Guid.NewGuid());
        var wikiPage = new WikiPage(project.Id, "WikiPage", "Content", Guid.NewGuid());
        project.AddWikiPage(wikiPage);
        _projectRepository.GetByIdWithWikiPagesAsync(
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

    [Fact]
    public async Task Handle_WhenMovingRootPage_ShouldCloseGapLeftBehind()
    {
        // Arrange
        var project = new Project("Project", "Description", Guid.NewGuid());
        var wikiPage1 = new WikiPage(project.Id, "WikiPage 1", "Content", Guid.NewGuid());
        var wikiPage2 = new WikiPage(project.Id, "WikiPage 2", "Content", Guid.NewGuid());
        var wikiPage3 = new WikiPage(project.Id, "WikiPage 3", "Content", Guid.NewGuid());
        project.AddWikiPage(wikiPage1);
        project.AddWikiPage(wikiPage2);
        project.AddWikiPage(wikiPage3);
        _projectRepository.GetByIdWithWikiPagesAsync(
            Command.ProjectId,
            TestContext.Current.CancellationToken)
            .Returns(project);

        // Act: move the first page to the end
        var result = await _handler.Handle(
            Command with { WikiPageId = wikiPage1.Id, TargetPosition = 2 },
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        wikiPage2.Position.Should().Be(0);
        wikiPage3.Position.Should().Be(1);
        wikiPage1.Position.Should().Be(2);
    }

    [Fact]
    public async Task Handle_WhenMovingAlreadyNestedPageToADifferentParent_ShouldDetachFromOldParent()
    {
        // Arrange
        var project = new Project("Project", "Description", Guid.NewGuid());
        var oldParent = new WikiPage(project.Id, "Old Parent", "Content", Guid.NewGuid());
        var newParent = new WikiPage(project.Id, "New Parent", "Content", Guid.NewGuid());
        var child = new WikiPage(project.Id, "Child", "Content", Guid.NewGuid());
        var sibling = new WikiPage(project.Id, "Sibling", "Content", Guid.NewGuid());
        project.AddWikiPage(oldParent);
        project.AddWikiPage(newParent);
        oldParent.InsertSubWikiPage(0, child);
        oldParent.InsertSubWikiPage(1, sibling);
        _projectRepository.GetByIdWithWikiPagesAsync(
            Command.ProjectId,
            TestContext.Current.CancellationToken)
            .Returns(project);

        // Act
        var result = await _handler.Handle(
            Command with { WikiPageId = child.Id, TargetParentId = newParent.Id },
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        oldParent.SubWikiPages.Should().ContainSingle().Which.Should().Be(sibling);
        sibling.Position.Should().Be(0);
        newParent.SubWikiPages.Should().ContainSingle().Which.Should().Be(child);
    }

    [Fact]
    public async Task Handle_WhenReorderingNestedSiblings_ShouldUpdatePositions()
    {
        // Arrange
        var project = new Project("Project", "Description", Guid.NewGuid());
        var parent = new WikiPage(project.Id, "Parent", "Content", Guid.NewGuid());
        var first = new WikiPage(project.Id, "First", "Content", Guid.NewGuid());
        var second = new WikiPage(project.Id, "Second", "Content", Guid.NewGuid());
        project.AddWikiPage(parent);
        parent.InsertSubWikiPage(0, first);
        parent.InsertSubWikiPage(1, second);
        _projectRepository.GetByIdWithWikiPagesAsync(
            Command.ProjectId,
            TestContext.Current.CancellationToken)
            .Returns(project);

        // Act: move "second" to be before "first", within the same parent
        var result = await _handler.Handle(
            Command with
            {
                WikiPageId = second.Id,
                TargetParentId = parent.Id,
                TargetPosition = 0,
            },
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        parent.SubWikiPages.Should().HaveCount(2);
        second.Position.Should().Be(0);
        first.Position.Should().Be(1);
    }
}