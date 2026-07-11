using Application.Abstractions.Authorization;
using Application.IntegrationTests.Infrastructure;
using Application.TagImportJobs.Queries.GetTagImportJob;
using Domain.Authorization;
using Domain.Organizations;
using Domain.Projects;
using Domain.TagImportJobs;
using Domain.Users;
using FluentAssertions;

namespace Application.IntegrationTests.TagImportJobs.Queries;

public class GetTagImportJobTests : BaseIntegrationTest
{
    public GetTagImportJobTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GetTagImportJob_WhenCallerHasNoAccessToProject_ShouldReturnForbidden()
    {
        // Arrange
        await SeedCurrentUserAsync();
        var organization = new Organization($"Organization {Guid.NewGuid()}", "Description");
        var project = new Project($"Project {Guid.NewGuid()}", "Description", organization.Id);
        _dbContext.Organizations.Add(organization);
        _dbContext.Projects.Add(project);

        var importJob = new TagImportJob(project.Id, "tags.csv", [1, 2, 3]);
        _dbContext.TagImportJobs.Add(importJob);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var query = new GetTagImportJobQuery(project.Id, importJob.Id);

        // Act
        var result = await _sender.Send(query, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(AuthorizationErrors.Forbidden);
    }

    [Fact]
    public async Task GetTagImportJob_WhenImportJobBelongsToDifferentProject_ShouldReturnNotFound()
    {
        // Arrange
        var currentUser = await SeedCurrentUserAsync();
        var organization = new Organization($"Organization {Guid.NewGuid()}", "Description");
        var ownProject = new Project($"Project {Guid.NewGuid()}", "Description", organization.Id);
        var otherProject = new Project($"Project {Guid.NewGuid()}", "Description", organization.Id);
        _dbContext.Organizations.Add(organization);
        _dbContext.Projects.AddRange(ownProject, otherProject);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        ownProject.AddMember(currentUser, await GetRoleAsync(SystemRoles.ProjectViewerId));

        var importJobInOtherProject = new TagImportJob(otherProject.Id, "tags.csv", [1, 2, 3]);
        _dbContext.TagImportJobs.Add(importJobInOtherProject);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var query = new GetTagImportJobQuery(ownProject.Id, importJobInOtherProject.Id);

        // Act
        var result = await _sender.Send(query, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(TagImportJobErrors.NotFound);
    }

    [Fact]
    public async Task GetTagImportJob_WhenCallerHasAccessAndJobBelongsToProject_ShouldReturnImportJob()
    {
        // Arrange
        var currentUser = await SeedCurrentUserAsync();
        var organization = new Organization($"Organization {Guid.NewGuid()}", "Description");
        var project = new Project($"Project {Guid.NewGuid()}", "Description", organization.Id);
        _dbContext.Organizations.Add(organization);
        _dbContext.Projects.Add(project);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        project.AddMember(currentUser, await GetRoleAsync(SystemRoles.ProjectViewerId));

        var importJob = new TagImportJob(project.Id, "tags.csv", [1, 2, 3]);
        _dbContext.TagImportJobs.Add(importJob);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var query = new GetTagImportJobQuery(project.Id, importJob.Id);

        // Act
        var result = await _sender.Send(query, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Id.Should().Be(importJob.Id);
        result.Value.FileName.Should().Be("tags.csv");
    }
}
