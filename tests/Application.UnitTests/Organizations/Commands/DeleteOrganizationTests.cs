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
        _mockOrganizationsRepository.GetByIdAsync(organization.Id, TestContext.Current.CancellationToken)
            .Returns(organization);
        var command = new DeleteOrganizationCommand(organization.Id);

        // Act
        var result = await _handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WhenOrganizationDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        _mockOrganizationsRepository.GetByIdAsync(Arg.Any<Guid>(), TestContext.Current.CancellationToken)
            .Returns(null as Organization);
        var command = new DeleteOrganizationCommand(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(OrganizationErrors.NotFound);
    }
}