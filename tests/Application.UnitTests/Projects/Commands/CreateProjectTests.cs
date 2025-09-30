using Application.Abstractions.Authentication;
using Application.Projects.Commands.CreateProject;
using Domain.Organizations;
using Domain.Projects;
using Domain.Users;

namespace Application.UnitTests.Projects.Commands;

public class CreateProjectTests
{
    private static readonly CreateProjectCommand Command = new(
        "Test Project",
        "Test Description",
        Guid.NewGuid());

    private readonly CreateProjectCommandHandler _handler;
    private readonly IOrganizationRepository _mockOrganizationRepository;
    private readonly IUserRepository _mockUserRepository;
    private readonly IUserContext _mockUserContext;

    public CreateProjectTests()
    {
        _mockOrganizationRepository = Substitute.For<IOrganizationRepository>();
        _mockUserRepository = Substitute.For<IUserRepository>();
        _mockUserContext = Substitute.For<IUserContext>();
        _handler = new CreateProjectCommandHandler(
            _mockOrganizationRepository,
            _mockUserRepository,
            _mockUserContext);
    }

    [Fact]
    public async Task Handle_WhenOrganizationDoesNotExist_ShouldReturnError()
    {
        // Arrange
        _mockOrganizationRepository.GetByIdWithProjectsAsync(
            Command.OrganizationId,
            TestContext.Current.CancellationToken)
            .Returns(null as Organization);

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeOfType<ErrorOr<Guid>>();
        result.FirstError.Should().Be(OrganizationErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenOrganizationExists_ShouldReturnProject()
    {
        // Arrange
        var organization = new Organization("Test Organization", "Test Description");
        _mockOrganizationRepository.GetByIdWithProjectsAsync(
            Command.OrganizationId,
            TestContext.Current.CancellationToken)
            .Returns(organization);

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

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
        _mockOrganizationRepository.GetByIdWithProjectsAsync(
            Command.OrganizationId,
            TestContext.Current.CancellationToken)
            .Returns(organization);

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeOfType<ErrorOr<Guid>>();
        result.FirstError.Should().Be(ProjectErrors.AlreadyExists);
    }
}