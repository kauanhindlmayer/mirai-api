using Application.Organizations.Commands.UpdateOrganization;
using Domain.Organizations;

namespace Application.UnitTests.Organizations.Commands;

public class UpdateOrganizationTests
{
    private static readonly UpdateOrganizationCommand Command = new(
        Guid.NewGuid(),
        "Updated Organization",
        "Updated Description");

    private readonly UpdateOrganizationCommandHandler _handler;
    private readonly IOrganizationsRepository _mockOrganizationsRepository;

    public UpdateOrganizationTests()
    {
        _mockOrganizationsRepository = Substitute.For<IOrganizationsRepository>();
        _handler = new UpdateOrganizationCommandHandler(_mockOrganizationsRepository);
    }

    [Fact]
    public async Task Handle_WhenOrganizationExists_UpdatesOrganization()
    {
        // Arrange
        var organization = new Organization("Test Organization", "Test Description");
        _mockOrganizationsRepository.GetByIdAsync(Command.Id, TestContext.Current.CancellationToken)
            .Returns(organization);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeEmpty();
        _mockOrganizationsRepository.Received().Update(organization);
    }

    [Fact]
    public async Task Handle_WhenOrganizationDoesNotExist_ReturnsError()
    {
        // Arrange
        _mockOrganizationsRepository.GetByIdAsync(Command.Id, TestContext.Current.CancellationToken)
            .Returns(null as Organization);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(OrganizationErrors.NotFound);
    }
}