using Application.Common.Interfaces.Persistence;
using Application.Projects.Commands.CreateProject;
using Domain.Organizations;
using Domain.Projects;

namespace Application.UnitTests.Projects.Commands;

public class CreateProjectTests
{
    private readonly CreateProjectCommandHandler _handler;
    private readonly IOrganizationsRepository _mockOrganizationsRepository;

    public CreateProjectTests()
    {
        _mockOrganizationsRepository = Substitute.For<IOrganizationsRepository>();
        _handler = new CreateProjectCommandHandler(_mockOrganizationsRepository);
    }

    [Fact]
    public async Task Handle_WhenOrganizationDoesNotExist_ShouldReturnError()
    {
        // Arrange
        _mockOrganizationsRepository.GetByIdWithProjectsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(null as Organization);
        var command = new CreateProjectCommand(
            "Test Project",
            "Test Description",
            Guid.NewGuid());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<ErrorOr<Project>>();
        result.Errors.First().Should().Be(OrganizationErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenOrganizationExists_ShouldReturnProject()
    {
        // Arrange
        var organization = new Organization("Test Organization", "Test Description");
        _mockOrganizationsRepository.GetByIdWithProjectsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(organization);
        var command = new CreateProjectCommand(
            "Test Project",
            "Test Description",
            organization.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<ErrorOr<Project>>();
        result.Value.Name.Should().Be(command.Name);
        result.Value.Description.Should().Be(command.Description);
        result.Value.OrganizationId.Should().Be(organization.Id);
    }

    [Fact]
    public async Task Handle_WhenOrganizationExistsAndProjectAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var organization = new Organization("Test Organization", "Test Description");
        var project = new Project("Test Project", "Test Description", organization.Id);
        organization.AddProject(project);
        _mockOrganizationsRepository.GetByIdWithProjectsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(organization);
        var command = new CreateProjectCommand(
            "Test Project",
            "Test Description",
            organization.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<ErrorOr<Project>>();
        result.Errors.First().Should().Be(ProjectErrors.AlreadyExists);
    }
}