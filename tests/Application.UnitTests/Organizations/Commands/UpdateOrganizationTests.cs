using Application.Common.Interfaces.Persistence;
using Application.Organizations.Commands.UpdateOrganization;
using Domain.Organizations;

namespace Application.UnitTests.Organizations.Commands;

public class UpdateOrganizationTests
{
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
        _mockOrganizationsRepository.GetByIdAsync(organization.Id, Arg.Any<CancellationToken>())
            .Returns(organization);

        var command = new UpdateOrganizationCommand(
            organization.Id,
            "Updated Organization",
            "Updated Description");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeEmpty();
        _mockOrganizationsRepository.Received().Update(organization);
    }

    [Fact]
    public async Task Handle_WhenOrganizationDoesNotExist_ReturnsError()
    {
        // Arrange
        var command = new UpdateOrganizationCommand(
            Guid.NewGuid(),
            "Updated Organization",
            "Updated Description");

        _mockOrganizationsRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(null as Organization);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(OrganizationErrors.NotFound);
    }
}