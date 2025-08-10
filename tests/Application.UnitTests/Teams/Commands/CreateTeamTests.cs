using Application.Teams.Commands.CreateTeam;
using Domain.Projects;
using Domain.Teams;

namespace Application.UnitTests.Teams.Commands;

public class CreateTeamTests
{
    private static readonly CreateTeamCommand Command = new(
        Guid.NewGuid(),
        "Name",
        "Description");

    private readonly CreateTeamCommandHandler _handler;
    private readonly IProjectRepository _projectRepository;

    public CreateTeamTests()
    {
        _projectRepository = Substitute.For<IProjectRepository>();
        _handler = new CreateTeamCommandHandler(_projectRepository);
    }

    [Fact]
    public async Task Handle_WhenProjectDoesNotExist_ShouldReturnError()
    {
        // Arrange
        _projectRepository.GetByIdAsync(Command.ProjectId, TestContext.Current.CancellationToken)
            .Returns(null as Project);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(ProjectErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenTeamWithSameNameAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var project = new Project("Name", "Description", Guid.NewGuid());
        project.AddTeam(new Team(project.Id, Command.Name, Command.Description));
        _projectRepository.GetByIdAsync(Command.ProjectId, TestContext.Current.CancellationToken)
            .Returns(project);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(ProjectErrors.TeamWithSameNameAlreadyExists);
    }

    [Fact]
    public async Task Handle_WhenProjectExists_ShouldCreateTeam()
    {
        // Arrange
        var project = new Project("Name", "Description", Guid.NewGuid());
        _projectRepository.GetByIdAsync(Command.ProjectId, TestContext.Current.CancellationToken)
            .Returns(project);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeEmpty();
        _projectRepository.Received().Update(project);
    }
}