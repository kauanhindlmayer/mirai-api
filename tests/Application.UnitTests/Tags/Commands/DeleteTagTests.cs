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
    private readonly IProjectRepository _projectRepository;
    private readonly ITagRepository _tagsRepository;

    public DeleteTagTests()
    {
        _projectRepository = Substitute.For<IProjectRepository>();
        _tagsRepository = Substitute.For<ITagRepository>();

        _handler = new DeleteTagCommandHandler(
            _projectRepository,
            _tagsRepository);
    }

    [Fact]
    public async Task Handle_WhenProjectDoesNotExist_ShouldReturnError()
    {
        // Arrange
        _projectRepository.GetByIdWithTagsAsync(
            Command.ProjectId,
            TestContext.Current.CancellationToken)
            .Returns(null as Project);

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(ProjectErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenTagDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var project = new Project("Project", "Description", Guid.NewGuid());
        _projectRepository.GetByIdWithTagsAsync(
            Command.ProjectId,
            TestContext.Current.CancellationToken)
            .Returns(project);

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

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
        _projectRepository.GetByIdWithTagsAsync(
            Command.ProjectId,
            TestContext.Current.CancellationToken)
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
        _projectRepository.GetByIdWithTagsAsync(
            Command.ProjectId,
            TestContext.Current.CancellationToken)
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