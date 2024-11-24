using Application.Common.Interfaces.Persistence;
using Application.Projects.Queries.GetProject;
using Domain.Projects;

namespace Application.UnitTests.Projects.Queries;

public class GetProjectTests
{
    private readonly GetProjectQueryHandler _handler;
    private readonly IProjectsRepository _mockProjectsRepository;

    public GetProjectTests()
    {
        _mockProjectsRepository = Substitute.For<IProjectsRepository>();
        _handler = new GetProjectQueryHandler(_mockProjectsRepository);
    }

    [Fact]
    public async Task Handle_WhenProjectExists_ShouldReturnProject()
    {
        // Arrange
        var project = new Project("Project", "Description", Guid.NewGuid());
        var query = new GetProjectQuery(project.Id);
        _mockProjectsRepository.GetByIdAsync(project.Id, Arg.Any<CancellationToken>())
            .Returns(project);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be(project);
    }

    [Fact]
    public async Task Handle_WhenProjectDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var query = new GetProjectQuery(Guid.NewGuid());
        _mockProjectsRepository.GetByIdAsync(query.ProjectId, Arg.Any<CancellationToken>())
            .Returns(null as Project);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(ProjectErrors.NotFound);
    }
}