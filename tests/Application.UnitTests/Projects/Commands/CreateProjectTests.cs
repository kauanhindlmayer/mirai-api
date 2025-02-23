using Application.Common.Interfaces.Persistence;
using Application.Projects.Commands.CreateProject;
using Domain.Organizations;
using Domain.Projects;

namespace Application.UnitTests.Projects.Commands;

public class CreateProjectTests
{
    private static readonly CreateProjectCommand Command = new(
        "Test Project",
        "Test Description",
        Guid.NewGuid());

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
        _mockOrganizationsRepository.GetByIdWithProjectsAsync(Command.OrganizationId, Arg.Any<CancellationToken>())
            .Returns(null as Organization);

        // Act
        var result = await _handler.Handle(Command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<ErrorOr<Guid>>();
        result.FirstError.Should().Be(OrganizationErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenOrganizationExists_ShouldReturnProject()
    {
        // Arrange
        var organization = new Organization("Test Organization", "Test Description");
        _mockOrganizationsRepository.GetByIdWithProjectsAsync(Command.OrganizationId, Arg.Any<CancellationToken>())
            .Returns(organization);

        // Act
        var result = await _handler.Handle(Command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<ErrorOr<Guid>>();
        result.Value.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_WhenOrganizationExistsAndProjectAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var organization = new Organization("Test Organization", "Test Description");
        var project = new Project("Test Project", "Test Description", organization.Id);
        organization.AddProject(project);
        _mockOrganizationsRepository.GetByIdWithProjectsAsync(Command.OrganizationId, Arg.Any<CancellationToken>())
            .Returns(organization);

        // Act
        var result = await _handler.Handle(Command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<ErrorOr<Guid>>();
        result.FirstError.Should().Be(ProjectErrors.AlreadyExists);
    }
}