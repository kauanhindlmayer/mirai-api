using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Services;
using Application.WorkItems.Commands.CreateWorkItem;
using Domain.Boards;
using Domain.Projects;
using Domain.Teams;
using Domain.WorkItems.Enums;

namespace Application.UnitTests.WorkItems.Commands;

public class CreateWorkItemTests
{
    private static readonly CreateWorkItemCommand Command = new(
        Guid.NewGuid(),
        WorkItemType.UserStory,
        "Title",
        Guid.NewGuid());

    private readonly CreateWorkItemCommandHandler _handler;
    private readonly IProjectsRepository _projectsRepository;
    private readonly IWorkItemsRepository _workItemsRepository;
    private readonly INlpService _nlpService;

    public CreateWorkItemTests()
    {
        _projectsRepository = Substitute.For<IProjectsRepository>();
        _workItemsRepository = Substitute.For<IWorkItemsRepository>();
        _nlpService = Substitute.For<INlpService>();
        _handler = new CreateWorkItemCommandHandler(
            _projectsRepository,
            _workItemsRepository,
            _nlpService);
    }

    [Fact]
    public async Task Handle_WhenProjectDoesNotExist_ShouldReturnError()
    {
        // Arrange
        _projectsRepository.GetByIdAsync(Command.ProjectId, TestContext.Current.CancellationToken)
            .Returns(null as Project);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(ProjectErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenProjectExists_ShouldReturnWorkItem()
    {
        // Arrange
        var project = new Project("Project", "Description", Guid.NewGuid());
        var team = new Team(project.Id, "Team", "Description");
        var board = new Board(team.Id, "Board");
        project.AddTeam(team);
        team.AddBoard(board);
        _projectsRepository.GetByIdWithTeamsAsync(Command.ProjectId, TestContext.Current.CancellationToken)
            .Returns(project);
        _workItemsRepository.GetNextWorkItemCodeAsync(Command.ProjectId, TestContext.Current.CancellationToken)
            .Returns(1);

        // Act
        var result = await _handler.Handle(
            Command with { AssignedTeamId = team.Id },
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_WhenWorkItemIsAddedToProject_ShouldUpdateProject()
    {
        // Arrange
        var project = new Project("Project", "Description", Guid.NewGuid());
        var team = new Team(project.Id, "Team", "Description");
        var board = new Board(team.Id, "Board");
        project.AddTeam(team);
        team.AddBoard(board);
        _projectsRepository.GetByIdWithTeamsAsync(Command.ProjectId, TestContext.Current.CancellationToken)
            .Returns(project);
        _workItemsRepository.GetNextWorkItemCodeAsync(Command.ProjectId, TestContext.Current.CancellationToken)
            .Returns(1);

        // Act
        await _handler.Handle(
            Command with { AssignedTeamId = team.Id },
            TestContext.Current.CancellationToken);

        // Assert
        _projectsRepository.Received().Update(project);
    }
}
