using Application.Projects.Commands.DisconnectGitHubRepository;
using Domain.Projects;

namespace Application.UnitTests.Projects.Commands;

public class DisconnectGitHubRepositoryTests
{
    private static readonly Guid OrganizationId = Guid.NewGuid();
    private static readonly Guid ProjectId = Guid.NewGuid();

    private static readonly DisconnectGitHubRepositoryCommand Command = new(OrganizationId, ProjectId);

    private readonly DisconnectGitHubRepositoryCommandHandler _handler;
    private readonly IProjectRepository _projectRepository;

    public DisconnectGitHubRepositoryTests()
    {
        _projectRepository = Substitute.For<IProjectRepository>();
        _handler = new DisconnectGitHubRepositoryCommandHandler(_projectRepository);
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
    public async Task Handle_WhenNoConnectionExists_ShouldReturnError()
    {
        // Arrange
        var project = new Project("Test Project", "Description", OrganizationId);
        _projectRepository.GetByIdWithGitHubRepositoryConnectionAsync(
            ProjectId,
            TestContext.Current.CancellationToken)
            .Returns(project);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(ProjectErrors.NoGitHubRepositoryConnected);
    }

    [Fact]
    public async Task Handle_WhenConnectionExists_ShouldDisconnect()
    {
        // Arrange
        var project = new Project("Test Project", "Description", OrganizationId);
        project.ConnectGitHubRepository(1001, 2002, "mirai-org", "mirai-app", Guid.NewGuid());
        _projectRepository.GetByIdWithGitHubRepositoryConnectionAsync(
            ProjectId,
            TestContext.Current.CancellationToken)
            .Returns(project);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        project.GitHubRepositoryConnection.Should().BeNull();
        _projectRepository.Received(1).Update(project);
    }
}
