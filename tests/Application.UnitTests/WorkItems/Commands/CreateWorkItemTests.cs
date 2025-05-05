using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Services;
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
        "Title",
        null);

    private readonly CreateWorkItemCommandHandler _handler;
    private readonly IProjectsRepository _projectsRepository;
    private readonly IWorkItemsRepository _workItemsRepository;
    private readonly ILanguageService _languageService;

    public CreateWorkItemTests()
    {
        _projectsRepository = Substitute.For<IProjectsRepository>();
        _workItemsRepository = Substitute.For<IWorkItemsRepository>();
        _languageService = Substitute.For<ILanguageService>();
        _handler = new CreateWorkItemCommandHandler(
            _projectsRepository,
            _workItemsRepository,
            _languageService);
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
        _projectsRepository.GetByIdAsync(Command.ProjectId, TestContext.Current.CancellationToken)
            .Returns(project);
        _workItemsRepository.GetNextWorkItemCodeAsync(Command.ProjectId, TestContext.Current.CancellationToken)
            .Returns(1);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_WhenWorkItemWithSameTitleAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var project = new Project("Project", "Description", Guid.NewGuid());
        var workItem = new WorkItem(Command.ProjectId, 1, "Title", WorkItemType.UserStory);
        project.AddWorkItem(workItem);
        _projectsRepository.GetByIdAsync(Command.ProjectId, TestContext.Current.CancellationToken)
            .Returns(project);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(ProjectErrors.WorkItemWithSameTitleAlreadyExists);
    }

    [Fact]
    public async Task Handle_WhenWorkItemIsAddedToProject_ShouldUpdateProject()
    {
        // Arrange
        var project = new Project("Project", "Description", Guid.NewGuid());
        _projectsRepository.GetByIdAsync(Command.ProjectId, TestContext.Current.CancellationToken)
            .Returns(project);
        _workItemsRepository.GetNextWorkItemCodeAsync(Command.ProjectId, TestContext.Current.CancellationToken)
            .Returns(1);

        // Act
        await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        _projectsRepository.Received().Update(project);
    }
}
