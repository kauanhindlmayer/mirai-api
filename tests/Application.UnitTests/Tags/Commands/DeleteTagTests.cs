using Application.Common.Interfaces.Persistence;
using Application.Tags.Commands.DeleteTag;
using Domain.Projects;
using Domain.Tags;
using Domain.WorkItems;
using Domain.WorkItems.Enums;

namespace Application.UnitTests.Tags.Commands;

public class DeleteTagTests
{
    private static readonly DeleteTagCommand Command = new(
        Guid.NewGuid(),
        Guid.NewGuid());

    private readonly DeleteTagCommandHandler _handler;
    private readonly IProjectsRepository _projectsRepository;
    private readonly ITagsRepository _tagsRepository;

    public DeleteTagTests()
    {
        _projectsRepository = Substitute.For<IProjectsRepository>();
        _tagsRepository = Substitute.For<ITagsRepository>();

        _handler = new DeleteTagCommandHandler(
            _projectsRepository,
            _tagsRepository);
    }

    [Fact]
    public async Task Handle_WhenProjectDoesNotExist_ShouldReturnError()
    {
        // Arrange
        _projectsRepository.GetByIdWithTagsAsync(Command.ProjectId, TestContext.Current.CancellationToken)
            .Returns(null as Project);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(ProjectErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenTagDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var project = new Project("Project", "Description", Guid.NewGuid());
        _projectsRepository.GetByIdWithTagsAsync(Command.ProjectId, TestContext.Current.CancellationToken)
            .Returns(project);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(TagErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenTagHasWorkItems_ShouldReturnError()
    {
        // Arrange
        var project = new Project("Project", "Description", Guid.NewGuid());
        var tag = new Tag("Tag", "Description", "#FFFFFF");
        var workItem = new WorkItem(Guid.NewGuid(), 1, "Title", WorkItemType.UserStory);
        tag.WorkItems.Add(workItem);
        workItem.AddTag(tag);
        project.Tags.Add(tag);
        _projectsRepository.GetByIdWithTagsAsync(Command.ProjectId, TestContext.Current.CancellationToken)
            .Returns(project);

        // Act
        var result = await _handler.Handle(
            Command with { TagId = tag.Id },
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(TagErrors.TagHasWorkItems);
    }

    [Fact]
    public async Task Handle_WhenCommandIsValid_ShouldRemoveTag()
    {
        // Arrange
        var project = new Project("Project", "Description", Guid.NewGuid());
        var tag = new Tag("Tag", "Description", "#FFFFFF");
        project.AddTag(tag);

        _projectsRepository.GetByIdWithTagsAsync(Command.ProjectId, TestContext.Current.CancellationToken)
            .Returns(project);

        // Act
        var result = await _handler.Handle(
                    Command with { TagId = tag.Id },
                    TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        project.Tags.Should().NotContain(tag);
    }
}