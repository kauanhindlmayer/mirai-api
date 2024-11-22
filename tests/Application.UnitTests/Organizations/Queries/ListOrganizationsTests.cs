using Application.Common.Interfaces.Persistence;
using Application.Organizations.Queries.ListOrganizations;
using Domain.Organizations;

namespace Application.UnitTests.Organizations.Queries;

public class ListOrganizationsTests
{
    private readonly ListOrganizationsQueryHandler _handler;
    private readonly IOrganizationsRepository _mockOrganizationsRepository;

    public ListOrganizationsTests()
    {
        _mockOrganizationsRepository = Substitute.For<IOrganizationsRepository>();
        _handler = new ListOrganizationsQueryHandler(_mockOrganizationsRepository);
    }

    [Fact]
    public async Task Handle_WhenOrganizationsExist_ShouldReturnOrganizations()
    {
        // Arrange
        List<Organization> organizations =
        [
            new Organization("Organization 1", "Description 1"),
            new Organization("Organization 2", "Description 2"),
        ];

        _mockOrganizationsRepository.ListAsync().ReturnsForAnyArgs(organizations);

        // Act
        var result = await _handler.Handle(new ListOrganizationsQuery(), default);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeEquivalentTo(organizations);
    }

    [Fact]
    public async Task Handle_WhenNoOrganizations_ShouldReturnEmptyList()
    {
        // Arrange
        _mockOrganizationsRepository.ListAsync().ReturnsForAnyArgs([]);

        // Act
        var result = await _handler.Handle(new ListOrganizationsQuery(), default);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeEmpty();
    }
}