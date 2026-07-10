using Application.Abstractions;
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
    private readonly IApplicationDbContext _mockContext;

    public CreateProjectTests()
    {
        _mockOrganizationRepository = Substitute.For<IOrganizationRepository>();
        _mockUserRepository = Substitute.For<IUserRepository>();
        _mockUserContext = Substitute.For<IUserContext>();
        _mockContext = Substitute.For<IApplicationDbContext>();
        _handler = new CreateProjectCommandHandler(
            _mockOrganizationRepository,
            _mockUserRepository,
            _mockUserContext,
            _mockContext);
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
    public async Task Handle_WhenOrganizationExistsAndProjectAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var organization = new Organization("Test Organization", "Test Description");
        var project = new Project("Test Project", "Test Description", organization.Id);
        var currentUser = new User("Test", "User", "test@example.com");
        organization.AddProject(project);
        _mockOrganizationRepository.GetByIdWithProjectsAsync(
            Command.OrganizationId,
            TestContext.Current.CancellationToken)
            .Returns(organization);
        _mockUserRepository.GetByIdAsync(
            _mockUserContext.UserId,
            TestContext.Current.CancellationToken)
            .Returns(currentUser);
        _mockOrganizationRepository.IsUserInOrganizationAsync(
            Command.OrganizationId,
            currentUser.Id,
            TestContext.Current.CancellationToken)
            .Returns(true);

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeOfType<ErrorOr<Guid>>();
        result.FirstError.Should().Be(ProjectErrors.AlreadyExists);
    }
}
