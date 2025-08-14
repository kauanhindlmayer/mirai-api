using Application.WorkItems.Commands.CreateWorkItem;
using Domain.Boards;
using Domain.Projects;
using Domain.Teams;
using Domain.WorkItems;
using Domain.WorkItems.Enums;
using Microsoft.Extensions.AI;

namespace Application.UnitTests.WorkItems.Commands;

public class CreateWorkItemTests
{
    private static readonly CreateWorkItemCommand Command = new(
        Guid.NewGuid(),
        WorkItemType.UserStory,
        "Title",
        Guid.NewGuid());

    private readonly CreateWorkItemCommandHandler _handler;
    private readonly IProjectRepository _projectRepository;
    private readonly IWorkItemRepository _workItemRepository;
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;
    private static readonly float[] Vector = [0.1f, 0.2f, 0.3f];

    public CreateWorkItemTests()
    {
        _projectRepository = Substitute.For<IProjectRepository>();
        _workItemRepository = Substitute.For<IWorkItemRepository>();
        _embeddingGenerator = Substitute.For<IEmbeddingGenerator<string, Embedding<float>>>();
        _handler = new CreateWorkItemCommandHandler(
            _projectRepository,
            _workItemRepository,
            _embeddingGenerator);
    }

    [Fact]
    public async Task Handle_WhenProjectDoesNotExist_ShouldReturnError()
    {
        // Arrange
        _projectRepository.GetByIdAsync(
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
    public async Task Handle_WhenProjectExists_ShouldReturnWorkItem()
    {
        // Arrange
        var project = new Project("Project", "Description", Guid.NewGuid());
        var team = new Team(project.Id, "Team", "Description");
        var board = new Board(team.Id, "Board");
        project.AddTeam(team);
        team.AddBoard(board);
        _projectRepository.GetByIdWithTeamsAsync(
            Command.ProjectId,
            TestContext.Current.CancellationToken)
            .Returns(project);
        _workItemRepository.GetNextWorkItemCodeAsync(
            Command.ProjectId,
            TestContext.Current.CancellationToken)
            .Returns(1);
        SetupEmbeddingGeneratorMock();

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
        _projectRepository.GetByIdWithTeamsAsync(
            Command.ProjectId,
            TestContext.Current.CancellationToken)
            .Returns(project);
        _workItemRepository.GetNextWorkItemCodeAsync(
            Command.ProjectId,
            TestContext.Current.CancellationToken)
            .Returns(1);
        SetupEmbeddingGeneratorMock();

        // Act
        await _handler.Handle(
            Command with { AssignedTeamId = team.Id },
            TestContext.Current.CancellationToken);

        // Assert
        _projectRepository.Received().Update(project);
    }

    private void SetupEmbeddingGeneratorMock()
    {
        var embedding = new GeneratedEmbeddings<Embedding<float>>([new(Vector)]);
        _embeddingGenerator
            .GenerateAsync(
                Arg.Any<IEnumerable<string>>(),
                Arg.Any<EmbeddingGenerationOptions?>(),
                TestContext.Current.CancellationToken)
            .Returns(embedding);
    }
}
