using Application.Common.Interfaces.Persistence;
using Application.WorkItems.Commands.CreateWorkItem;
using Domain.Projects;
using Domain.WorkItems;
using Domain.WorkItems.Enums;

namespace Application.UnitTests.WorkItems.Commands;

public class CreateWorkItemTests
{
    private static readonly CreateWorkItemCommand Command = new(
        Guid.NewGuid(),
        WorkItemType.UserStory,
        "Title");

    private readonly CreateWorkItemCommandHandler _handler;
    private readonly IProjectsRepository _projectsRepository;
    private readonly IWorkItemsRepository _workItemsRepository;

    public CreateWorkItemTests()
    {
        _projectsRepository = Substitute.For<IProjectsRepository>();
        _workItemsRepository = Substitute.For<IWorkItemsRepository>();
        _handler = new CreateWorkItemCommandHandler(
            _projectsRepository,
            _workItemsRepository);
    }

    [Fact]
    public async Task Handle_WhenProjectDoesNotExist_ReturnsProjectNotFoundError()
    {
        // Arrange
        _projectsRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(null as Project);

        // Act
        var result = await _handler.Handle(Command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.First().Should().BeEquivalentTo(ProjectErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenProjectExists_ReturnsWorkItem()
    {
        // Arrange
        var project = new Project("Project", "Description", Guid.NewGuid());
        _projectsRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(project);
        _workItemsRepository.GetNextWorkItemCodeAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        var result = await _handler.Handle(Command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.ProjectId.Should().Be(project.Id);
        result.Value.Code.Should().Be(1);
        result.Value.Title.Should().Be("Title");
        result.Value.Type.Should().Be(WorkItemType.UserStory);
    }

    [Fact]
    public async Task Handle_WhenWorkItemWithSameTitleAlreadyExists_ReturnsWorkItemWithSameTitleAlreadyExistsError()
    {
        // Arrange
        var project = new Project("Project", "Description", Guid.NewGuid());
        project.AddWorkItem(new WorkItem(project.Id, 1, "Title", WorkItemType.UserStory));
        _projectsRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(project);

        // Act
        var result = await _handler.Handle(Command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.First().Should().BeEquivalentTo(ProjectErrors.WorkItemWithSameTitleAlreadyExists);
    }

    [Fact]
    public async Task Handle_WhenWorkItemIsAddedToProject_UpdatesProject()
    {
        // Arrange
        var project = new Project("Project", "Description", Guid.NewGuid());
        _projectsRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(project);
        _workItemsRepository.GetNextWorkItemCodeAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        await _handler.Handle(Command, CancellationToken.None);

        // Assert
        _projectsRepository.Received().Update(project);
    }
}
