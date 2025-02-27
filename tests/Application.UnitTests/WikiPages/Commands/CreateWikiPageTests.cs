using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Services;
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
    private readonly IProjectsRepository _projectsRepository;
    private readonly IUserContext _userContext;
    private readonly IHtmlSanitizerService _htmlSanitizerService;

    public CreateWikiPageTests()
    {
        _projectsRepository = Substitute.For<IProjectsRepository>();
        _userContext = Substitute.For<IUserContext>();
        _htmlSanitizerService = Substitute.For<IHtmlSanitizerService>();

        _handler = new CreateWikiPageCommandHandler(
            _projectsRepository,
            _userContext,
            _htmlSanitizerService);
    }

    [Fact]
    public async Task Handle_WhenProjectDoesNotExist_ShouldReturnError()
    {
        // Arrange
        _projectsRepository.GetByIdWithWikiPagesAsync(Command.ProjectId, Arg.Any<CancellationToken>())
            .Returns(null as Project);

        // Act
        var result = await _handler.Handle(Command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(ProjectErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenParentWikiPageDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var project = new Project("Project", "Description", Guid.NewGuid());
        _projectsRepository.GetByIdWithWikiPagesAsync(Command.ProjectId, Arg.Any<CancellationToken>())
            .Returns(project);

        // Act
        var result = await _handler.Handle(
            Command with { ParentWikiPageId = Guid.NewGuid() },
            CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(WikiPageErrors.ParentWikiPageNotFound);
    }

    [Fact]
    public async Task Handle_WhenWikiPageWithSameTitleAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var project = new Project("Project", "Description", Guid.NewGuid());
        var wikiPage = new WikiPage(Guid.NewGuid(), Command.Title, Command.Content, Guid.NewGuid(), null);
        project.AddWikiPage(wikiPage);
        _projectsRepository.GetByIdWithWikiPagesAsync(Command.ProjectId, Arg.Any<CancellationToken>())
            .Returns(project);

        // Act
        var result = await _handler.Handle(Command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(ProjectErrors.WikiPageWithSameTitleAlreadyExists);
    }

    [Fact]
    public async Task Handle_WhenCommandIsValid_ShouldCreateWikiPage()
    {
        // Arrange
        var project = new Project("Project", "Description", Guid.NewGuid());
        _projectsRepository.GetByIdWithWikiPagesAsync(Command.ProjectId, Arg.Any<CancellationToken>())
            .Returns(project);
        _htmlSanitizerService.Sanitize(Command.Content).Returns(Command.Content);

        // Act
        var result = await _handler.Handle(Command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        project.WikiPages.Should().ContainSingle().Which.Title.Should().Be(Command.Title);
    }
}