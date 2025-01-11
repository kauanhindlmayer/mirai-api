using Application.Common.Interfaces.Persistence;
using Application.Projects.Commands.UpdateProject;
using Domain.Organizations;
using Domain.Projects;

namespace Application.UnitTests.Projects.Commands;

public class UpdateProjectTests
{
    private static readonly UpdateProjectCommand Command = new(
        Guid.NewGuid(),
        Guid.NewGuid(),
        "New Project Name",
        "New Test Description");

    private readonly UpdateProjectCommandHandler _handler;
    private readonly IOrganizationsRepository _mockOrganizationsRepository;

    public UpdateProjectTests()
    {
        _mockOrganizationsRepository = Substitute.For<IOrganizationsRepository>();
        _handler = new UpdateProjectCommandHandler(_mockOrganizationsRepository);
    }

    [Fact]
    public async Task Handle_WhenOrganizationDoesNotExist_ShouldReturnError()
    {
        // Arrange
        _mockOrganizationsRepository.GetByIdWithProjectsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(null as Organization);

        // Act
        var result = await _handler.Handle(Command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<ErrorOr<Guid>>();
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(OrganizationErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenOrganizationExistsAndProjectDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var organization = new Organization("Test Organization", "Test Description");
        _mockOrganizationsRepository.GetByIdWithProjectsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(organization);

        // Act
        var result = await _handler.Handle(Command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<ErrorOr<Guid>>();
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(ProjectErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenOrganizationExistsAndProjectExists_ShouldUpdateProject()
    {
        // Arrange
        var organization = new Organization("Test Organization", "Test Description");
        var project = new Project("Test Project", "Test Description", organization.Id);
        organization.AddProject(project);
        _mockOrganizationsRepository.GetByIdWithProjectsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(organization);
        var command = Command with { OrganizationId = organization.Id, ProjectId = project.Id };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<ErrorOr<Guid>>();
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeEmpty();
    }
}