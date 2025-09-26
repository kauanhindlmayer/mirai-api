using Application.Tags.Commands.CreateTag;
using Domain.Projects;
using Domain.Tags;

namespace Application.UnitTests.Tags.Commands;

public class CreateTagTests
{
    private static readonly CreateTagCommand Command = new(
        Guid.NewGuid(),
        "Test Tag",
        "Test Description",
        "#FFFFFF");

    private readonly CreateTagCommandHandler _handler;
    private readonly IProjectRepository _projectRepository;

    public CreateTagTests()
    {
        _projectRepository = Substitute.For<IProjectRepository>();
        _handler = new CreateTagCommandHandler(_projectRepository);
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
        result.FirstError.Should().Be(ProjectErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenTagAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var project = new Project("Test Project", "Test Description", Guid.NewGuid());
        var tag = new Tag("Test Tag", "Test Description", "#FFFFFF");
        project.AddTag(tag);
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
        result.FirstError.Should().Be(TagErrors.AlreadyExists);
    }

    [Fact]
    public async Task Handle_WhenCommandIsValid_ShouldCreateTag()
    {
        // Arrange
        var project = new Project("Test Project", "Test Description", Guid.NewGuid());
        _projectRepository.GetByIdWithTagsAsync(
            Command.ProjectId,
            TestContext.Current.CancellationToken)
            .Returns(project);

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        project.Tags.Should().ContainSingle();
        project.Tags.First().Name.Should().Be(Command.Name);
        project.Tags.First().Description.Should().Be(Command.Description);
        project.Tags.First().Color.Should().Be(Command.Color);
        _projectRepository.Received(1).Update(project);
    }
}