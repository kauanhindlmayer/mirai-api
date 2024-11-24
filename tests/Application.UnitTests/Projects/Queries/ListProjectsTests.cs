using Application.Common.Interfaces.Persistence;
using Application.Projects.Queries.ListProjects;
using Domain.Organizations;
using Domain.Projects;

namespace Application.UnitTests.Projects.Queries;

public class ListProjectsTests
{
    private readonly ListProjectsQueryHandler _handler;
    private readonly IOrganizationsRepository _mockOrganizationsRepository;

    public ListProjectsTests()
    {
        _mockOrganizationsRepository = Substitute.For<IOrganizationsRepository>();
        _handler = new ListProjectsQueryHandler(_mockOrganizationsRepository);
    }

    [Fact]
    public async Task Handle_WhenOrganizationNotFound_ReturnsOrganizationErrorsNotFound()
    {
        // Arrange
        var query = new ListProjectsQuery(Guid.NewGuid());
        _mockOrganizationsRepository.GetByIdWithProjectsAsync(
            query.OrganizationId,
            Arg.Any<CancellationToken>())
            .Returns(null as Organization);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(OrganizationErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenOrganizationFound_ReturnsProjects()
    {
        // Arrange
        var organization = new Organization("Organization Name", "Description");
        var project = new Project("Project Name", "Description", organization.Id);
        organization.Projects.Add(project);
        var query = new ListProjectsQuery(organization.Id);
        _mockOrganizationsRepository.GetByIdWithProjectsAsync(
            query.OrganizationId,
            Arg.Any<CancellationToken>())
            .Returns(organization);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeEquivalentTo(organization.Projects.ToList());
    }
}