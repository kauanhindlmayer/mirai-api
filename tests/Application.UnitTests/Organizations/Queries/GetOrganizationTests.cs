using Application.Common.Interfaces.Persistence;
using Application.Organizations.Queries.GetOrganization;
using Domain.Organizations;

namespace Application.UnitTests.Organizations.Queries;

public class GetOrganizationTests
{
    private readonly GetOrganizationQueryHandler _handler;
    private readonly IOrganizationsRepository _mockOrganizationsRepository;

    public GetOrganizationTests()
    {
        _mockOrganizationsRepository = Substitute.For<IOrganizationsRepository>();
        _handler = new GetOrganizationQueryHandler(_mockOrganizationsRepository);
    }

    [Fact]
    public async Task Handle_WhenOrganizationExists_ShouldReturnOrganization()
    {
        // Arrange
        var organization = new Organization("Test Organization", "Test Description");
        _mockOrganizationsRepository.GetByIdAsync(organization.Id, Arg.Any<CancellationToken>())
            .Returns(organization);
        var query = new GetOrganizationQuery(organization.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Should().BeOfType<ErrorOr<Organization>>();
        result.Value.Name.Should().Be(organization.Name);
        result.Value.Description.Should().Be(organization.Description);
    }

    [Fact]
    public async Task Handle_WhenOrganizationDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var query = new GetOrganizationQuery(Guid.NewGuid());
        _mockOrganizationsRepository.GetByIdAsync(query.OrganizationId, Arg.Any<CancellationToken>())
            .Returns(null as Organization);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.Should().BeOfType<ErrorOr<Organization>>();
        result.FirstError.Should().Be(OrganizationErrors.NotFound);
    }
}