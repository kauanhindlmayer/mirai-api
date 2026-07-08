using Application.Abstractions.Authentication;
using Application.Projects.Commands.ConnectGitHubRepository;
using Domain.Projects;

namespace Application.UnitTests.Projects.Commands;

public class ConnectGitHubRepositoryTests
{
    private static readonly Guid OrganizationId = Guid.NewGuid();
    private static readonly Guid ProjectId = Guid.NewGuid();
    private static readonly Guid UserId = Guid.NewGuid();

    private static readonly ConnectGitHubRepositoryCommand Command = new(
        OrganizationId,
        ProjectId,
        InstallationId: 1001,
        RepositoryId: 2002,
        RepositoryOwner: "mirai-org",
        RepositoryName: "mirai-app");

    private readonly ConnectGitHubRepositoryCommandHandler _handler;
    private readonly IProjectRepository _projectRepository;
    private readonly IUserContext _userContext;

    public ConnectGitHubRepositoryTests()
    {
        _projectRepository = Substitute.For<IProjectRepository>();
        _userContext = Substitute.For<IUserContext>();
        _userContext.UserId.Returns(UserId);
        _handler = new ConnectGitHubRepositoryCommandHandler(_projectRepository, _userContext);
    }

    [Fact]
    public async Task Handle_WhenProjectDoesNotExist_ShouldReturnError()
    {
        // Arrange
        _projectRepository.GetByIdWithGitHubRepositoryConnectionAsync(
            ProjectId,
            TestContext.Current.CancellationToken)
            .Returns(null as Project);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(ProjectErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenProjectBelongsToDifferentOrganization_ShouldReturnError()
    {
        // Arrange
        var project = new Project("Test Project", "Description", Guid.NewGuid());
        _projectRepository.GetByIdWithGitHubRepositoryConnectionAsync(
            ProjectId,
            TestContext.Current.CancellationToken)
            .Returns(project);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(ProjectErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenRepositoryAlreadyConnectedToAnotherProject_ShouldReturnError()
    {
        // Arrange
        var project = new Project("Test Project", "Description", OrganizationId);
        _projectRepository.GetByIdWithGitHubRepositoryConnectionAsync(
            ProjectId,
            TestContext.Current.CancellationToken)
            .Returns(project);

        var otherProject = new Project("Other Project", "Description", OrganizationId);
        _projectRepository.GetByGitHubRepositoryIdAsync(
            Command.RepositoryId,
            TestContext.Current.CancellationToken)
            .Returns(otherProject);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(ProjectErrors.GitHubRepositoryAlreadyConnectedElsewhere);
    }

    [Fact]
    public async Task Handle_WhenRepositoryIsNotConnectedElsewhere_ShouldConnectRepository()
    {
        // Arrange
        var project = new Project("Test Project", "Description", OrganizationId);
        _projectRepository.GetByIdWithGitHubRepositoryConnectionAsync(
            ProjectId,
            TestContext.Current.CancellationToken)
            .Returns(project);
        _projectRepository.GetByGitHubRepositoryIdAsync(
            Command.RepositoryId,
            TestContext.Current.CancellationToken)
            .Returns(null as Project);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        project.GitHubRepositoryConnection.Should().NotBeNull();
        project.GitHubRepositoryConnection!.RepositoryId.Should().Be(Command.RepositoryId);
        project.GitHubRepositoryConnection!.ConnectedByUserId.Should().Be(UserId);
        _projectRepository.Received(1).Update(project);
    }
}
