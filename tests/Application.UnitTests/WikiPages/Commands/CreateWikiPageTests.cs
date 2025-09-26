using Application.Abstractions;
using Application.Abstractions.Authentication;
using Application.WikiPages.Commands.CreateWikiPage;
using Domain.Projects;
using Domain.WikiPages;

namespace Application.UnitTests.WikiPages.Commands;

public class CreateWikiPageTests
{
    private static readonly CreateWikiPageCommand Command = new(
        Guid.NewGuid(),
        "Title",
        "Content",
        null);

    private readonly CreateWikiPageCommandHandler _handler;
    private readonly IProjectRepository _projectRepository;
    private readonly IUserContext _userContext;
    private readonly IHtmlSanitizerService _htmlSanitizerService;

    public CreateWikiPageTests()
    {
        _projectRepository = Substitute.For<IProjectRepository>();
        _userContext = Substitute.For<IUserContext>();
        _htmlSanitizerService = Substitute.For<IHtmlSanitizerService>();

        _handler = new CreateWikiPageCommandHandler(
            _projectRepository,
            _userContext,
            _htmlSanitizerService);
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
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(ProjectErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenParentWikiPageDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var project = new Project("Project", "Description", Guid.NewGuid());
        _projectRepository.GetByIdWithWikiPagesAsync(
            Command.ProjectId,
            TestContext.Current.CancellationToken)
            .Returns(project);

        // Act
        var result = await _handler.Handle(
            Command with { ParentWikiPageId = Guid.NewGuid() },
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(WikiPageErrors.ParentWikiPageNotFound);
    }

    [Fact]
    public async Task Handle_WhenWikiPageWithSameTitleAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var project = new Project("Project", "Description", Guid.NewGuid());
        var wikiPage = new WikiPage(
            Guid.NewGuid(),
            Command.Title,
            Command.Content,
            Guid.NewGuid());
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
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(ProjectErrors.WikiPageWithSameTitleAlreadyExists);
    }

    [Fact]
    public async Task Handle_WhenCommandIsValid_ShouldCreateWikiPage()
    {
        // Arrange
        var project = new Project("Project", "Description", Guid.NewGuid());
        _projectRepository.GetByIdWithWikiPagesAsync(
            Command.ProjectId,
            TestContext.Current.CancellationToken)
            .Returns(project);
        _htmlSanitizerService.Sanitize(Command.Content).Returns(Command.Content);

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        project.WikiPages.Should().ContainSingle()
            .Which.Title.Should().Be(Command.Title);
    }
}