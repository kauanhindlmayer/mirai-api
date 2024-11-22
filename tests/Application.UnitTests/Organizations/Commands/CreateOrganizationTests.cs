using Application.Common.Interfaces.Persistence;
using Application.Organizations.Commands.CreateOrganization;
using Domain.Organizations;

namespace Application.UnitTests.Organizations.Commands;

public class CreateOrganizationTests
{
    private static readonly CreateOrganizationCommand Command = new(
        "Test Organization",
        "Test Description");

    private readonly CreateOrganizationCommandHandler _handler;
    private readonly IOrganizationsRepository _mockOrganizationsRepository;

    public CreateOrganizationTests()
    {
        _mockOrganizationsRepository = Substitute.For<IOrganizationsRepository>();
        _handler = new CreateOrganizationCommandHandler(_mockOrganizationsRepository);
    }

    [Fact]
    public async Task Handle_WhenOrganizationExists_ShouldReturnError()
    {
        // Arrange
        _mockOrganizationsRepository.ExistsByNameAsync(Command.Name, Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        var result = await _handler.Handle(Command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<ErrorOr<Organization>>();
        result.Errors.First().Should().Be(OrganizationErrors.AlreadyExists);
    }

    [Fact]
    public async Task Handle_WhenOrganizationDoesNotExist_ShouldReturnOrganization()
    {
        // Arrange
        _mockOrganizationsRepository.ExistsByNameAsync(Command.Name, Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var result = await _handler.Handle(Command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<ErrorOr<Organization>>();
        result.Value.Name.Should().Be(Command.Name);
        result.Value.Description.Should().Be(Command.Description);
    }
}