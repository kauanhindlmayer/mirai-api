using Application.Organizations.Commands.CreateOrganization;
using Domain.Organizations;

namespace Application.UnitTests.Organizations.Commands;

public class CreateOrganizationTests
{
    private static readonly CreateOrganizationCommand Command = new(
        "Test Organization",
        "Test Description");

    private readonly CreateOrganizationCommandHandler _handler;
    private readonly IOrganizationRepository _mockorganizationRepository;

    public CreateOrganizationTests()
    {
        _mockorganizationRepository = Substitute.For<IOrganizationRepository>();
        _handler = new CreateOrganizationCommandHandler(_mockorganizationRepository);
    }

    [Fact]
    public async Task Handle_WhenOrganizationExists_ShouldReturnError()
    {
        // Arrange
        _mockorganizationRepository.ExistsByNameAsync(Command.Name, TestContext.Current.CancellationToken)
            .Returns(true);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeOfType<ErrorOr<Guid>>();
        result.FirstError.Should().Be(OrganizationErrors.AlreadyExists);
    }

    [Fact]
    public async Task Handle_WhenOrganizationDoesNotExist_ShouldReturnOrganizationId()
    {
        // Arrange
        _mockorganizationRepository.ExistsByNameAsync(Command.Name, TestContext.Current.CancellationToken)
            .Returns(false);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeOfType<ErrorOr<Guid>>();
        result.Value.Should().NotBeEmpty();
    }
}