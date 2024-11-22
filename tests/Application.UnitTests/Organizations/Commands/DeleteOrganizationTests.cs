using Application.Common.Interfaces.Persistence;
using Application.Organizations.Commands.CreateOrganization;
using Application.Organizations.Commands.DeleteOrganization;
using Domain.Organizations;

namespace Application.UnitTests.Organizations.Commands;

public class DeleteOrganizationTests
{
    private static readonly CreateOrganizationCommand Command = new(
        "Test Organization",
        "Test Description");

    private readonly DeleteOrganizationCommandHandler _handler;
    private readonly IOrganizationsRepository _mockOrganizationsRepository;

    public DeleteOrganizationTests()
    {
        _mockOrganizationsRepository = Substitute.For<IOrganizationsRepository>();
        _handler = new DeleteOrganizationCommandHandler(_mockOrganizationsRepository);
    }

    [Fact]
    public async Task Handle_WhenOrganizationExists_ShouldDeleteOrganization()
    {
        // Arrange
        var organization = new Organization(Command.Name, Command.Description);
        _mockOrganizationsRepository.GetByIdAsync(organization.Id, Arg.Any<CancellationToken>())
            .Returns(organization);
        var command = new DeleteOrganizationCommand(organization.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WhenOrganizationDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        _mockOrganizationsRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(null as Organization);
        var command = new DeleteOrganizationCommand(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.First().Should().Be(OrganizationErrors.NotFound);
    }
}