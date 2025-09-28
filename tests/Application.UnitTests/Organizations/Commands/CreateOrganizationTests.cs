using Application.Abstractions.Authentication;
using Application.Organizations.Commands.CreateOrganization;
using Domain.Organizations;
using Domain.Users;

namespace Application.UnitTests.Organizations.Commands;

public class CreateOrganizationTests
{
    private static readonly CreateOrganizationCommand Command = new(
        "Test Organization",
        "Test Description");

    private readonly CreateOrganizationCommandHandler _handler;
    private readonly IOrganizationRepository _mockOrganizationRepository;
    private readonly IUserRepository _mockUserRepository;
    private readonly IUserContext _mockUserContext;

    public CreateOrganizationTests()
    {
        _mockOrganizationRepository = Substitute.For<IOrganizationRepository>();
        _mockUserRepository = Substitute.For<IUserRepository>();
        _mockUserContext = Substitute.For<IUserContext>();

        _handler = new CreateOrganizationCommandHandler(
            _mockOrganizationRepository,
            _mockUserRepository,
            _mockUserContext);
    }

    [Fact]
    public async Task Handle_WhenOrganizationExists_ShouldReturnError()
    {
        // Arrange
        _mockOrganizationRepository.ExistsByNameAsync(
            Command.Name,
            TestContext.Current.CancellationToken)
            .Returns(true);

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeOfType<ErrorOr<Guid>>();
        result.FirstError.Should().Be(OrganizationErrors.AlreadyExists);
    }

    [Fact]
    public async Task Handle_WhenOrganizationDoesNotExist_ShouldReturnOrganizationId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User("test@example.com", "Test", "User");

        _mockUserContext.UserId.Returns(userId);
        _mockUserRepository.GetByIdAsync(
            userId,
            TestContext.Current.CancellationToken)
            .Returns(user);

        _mockOrganizationRepository.ExistsByNameAsync(
            Command.Name,
            TestContext.Current.CancellationToken)
            .Returns(false);

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeOfType<ErrorOr<Guid>>();
        result.Value.Should().NotBeEmpty();

        await _mockOrganizationRepository.Received(1).AddAsync(
            Arg.Is<Organization>(o => o.Users.Contains(user)),
            TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ShouldReturnError()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _mockUserContext.UserId.Returns(userId);
        _mockUserRepository.GetByIdAsync(
            userId,
            TestContext.Current.CancellationToken)
            .Returns((User?)null);

        _mockOrganizationRepository.ExistsByNameAsync(
            Command.Name,
            TestContext.Current.CancellationToken)
            .Returns(false);

        // Act
        var result = await _handler.Handle(
            Command,
            TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeOfType<ErrorOr<Guid>>();
        result.FirstError.Should().Be(UserErrors.NotFound);
    }
}