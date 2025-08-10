using Application.Tags.Commands.UpdateTag;
using Domain.Projects;
using Domain.Tags;

namespace Application.UnitTests.Tags.Commands;

public class UpdateTagTests
{
    private static readonly UpdateTagCommand Command = new(
        Guid.NewGuid(),
        Guid.NewGuid(),
        "Name",
        "Description",
        "Color");

    private readonly UpdateTagCommandHandler _handler;
    private readonly IProjectRepository _projectRepository;

    public UpdateTagTests()
    {
        _projectRepository = Substitute.For<IProjectRepository>();
        _handler = new UpdateTagCommandHandler(_projectRepository);
    }

    [Fact]
    public async Task Handle_WhenProjectDoesNotExist_ShouldReturnError()
    {
        // Arrange
        _projectRepository.GetByIdWithTagsAsync(Command.ProjectId, TestContext.Current.CancellationToken)
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
        _projectRepository.GetByIdWithTagsAsync(Command.ProjectId, TestContext.Current.CancellationToken)
            .Returns(project);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(TagErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenTagExists_ShouldUpdateTag()
    {
        // Arrange
        var project = new Project("Project", "Description", Command.ProjectId);
        var tag = new Tag("Name", "Description", "Color");
        project.AddTag(tag);
        _projectRepository.GetByIdWithTagsAsync(Command.ProjectId, TestContext.Current.CancellationToken)
            .Returns(project);

        // Act
        var result = await _handler.Handle(
            Command with { TagId = tag.Id },
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be(tag.Id);
        tag.Name.Should().Be(Command.Name);
        tag.Description.Should().Be(Command.Description);
        tag.Color.Should().Be(Command.Color);
        _projectRepository.Received().Update(project);
    }
}