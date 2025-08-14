using Application.Organizations.Commands.CreateOrganization;
using Domain.Organizations;

namespace Application.UnitTests.Organizations.Commands;

public class CreateOrganizationTests
{
    private static readonly CreateOrganizationCommand Command = new(
        "Test Organization",
        "Test Description");

    private readonly CreateOrganizationCommandHandler _handler;
    private readonly IOrganizationRepository _mockOrganizationRepository;

    public CreateOrganizationTests()
    {
        _mockOrganizationRepository = Substitute.For<IOrganizationRepository>();
        _handler = new CreateOrganizationCommandHandler(_mockOrganizationRepository);
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
    }
}