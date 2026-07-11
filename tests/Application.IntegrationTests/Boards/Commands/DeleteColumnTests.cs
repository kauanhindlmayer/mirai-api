using Application.Boards.Commands.DeleteColumn;
using Application.IntegrationTests.Infrastructure;
using Domain.Authorization;
using Domain.Boards;
using Domain.Organizations;
using Domain.Projects;
using Domain.Teams;
using Domain.Users;
using Domain.WorkItems;
using Domain.WorkItems.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Application.IntegrationTests.Boards.Commands;

public class DeleteColumnTests : BaseIntegrationTest
{
    public DeleteColumnTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task DeleteColumn_WhenColumnHasCards_ShouldReturnError()
    {
        // Arrange
        await SeedCurrentUserAsync();
        var organization = new Organization($"Organization {Guid.NewGuid()}", "Description");
        var project = new Project($"Project {Guid.NewGuid()}", "Description", organization.Id);
        var team = new Team(project.Id, "Engineering", "Description");
        var admin = new User("Admin", "User", $"admin-{Guid.NewGuid()}@mirai.com");
        admin.SetIdentityId(Guid.NewGuid().ToString());
        _dbContext.Organizations.Add(organization);
        _dbContext.Projects.Add(project);
        _dbContext.Teams.Add(team);
        _dbContext.Users.Add(admin);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        team.AddMember(admin, await GetRoleAsync(SystemRoles.TeamAdminId));

        var board = await _dbContext.Boards.SingleAsync(
            b => b.TeamId == team.Id,
            TestContext.Current.CancellationToken);
        var column = board.DefaultColumn;
        var workItem = new WorkItem(project.Id, 1, "Title", WorkItemType.UserStory);
        _dbContext.WorkItems.Add(workItem);
        var card = new BoardCard(column.Id, workItem.Id);
        column.AddCard(card);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        SetCurrentUser(admin.Id);
        var command = new DeleteColumnCommand(board.Id, column.Id);

        _dbContext.ChangeTracker.Clear();

        // Act
        var result = await _sender.Send(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(BoardErrors.ColumnHasCards(column));
    }
}
