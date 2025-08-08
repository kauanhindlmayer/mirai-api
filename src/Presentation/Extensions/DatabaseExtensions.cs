using Bogus;
using Domain.Boards;
using Domain.Organizations;
using Domain.Projects;
using Domain.Sprints;
using Domain.Teams;
using Domain.Users;
using Domain.WikiPages;
using Domain.WorkItems;
using Domain.WorkItems.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Presentation.Extensions;

public static class DatabaseExtensions
{
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        try
        {
            using var scope = app.Services.CreateScope();
            await using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            await dbContext.Database.MigrateAsync();
            app.Logger.LogInformation("Database migrations applied successfully");
        }
        catch (Exception exception)
        {
            app.Logger.LogError(exception, "An error occurred while applying database migrations");
            throw;
        }
    }

    public static async Task SeedDataAsync(this WebApplication app)
    {
        try
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            app.Logger.LogInformation("Starting database seeding...");

            if (context.Organizations.Any())
            {
                app.Logger.LogInformation("Database already seeded. Skipping seeding process.");
                return;
            }

            var faker = new Faker();

            var organization = new Organization(
                faker.Company.CompanyName(),
                faker.Company.CatchPhrase());
            await context.Organizations.AddAsync(organization);

            var project = new Project(
                faker.Commerce.ProductName(),
                faker.Commerce.ProductDescription(),
                organization.Id);
            await context.Projects.AddAsync(project);

            var team = new Team(
                project.Id,
                project.Name,
                faker.Lorem.Sentence());
            await context.Teams.AddAsync(team);

            var startDate = faker.Date.Recent().ToUniversalTime();
            var sprint = new Sprint(
                team.Id,
                "Sprint 1",
                DateOnly.FromDateTime(startDate),
                DateOnly.FromDateTime(startDate.AddDays(14)));
            await context.Sprints.AddAsync(sprint);

            var board = new Board(
                team.Id,
                team.Name);
            await context.Boards.AddAsync(board);

            var users = await SeedUsers(context, organization, project);
            await SeedWikiPages(faker, context, project, users);
            await SeedWorkItems(faker, context, project, team, sprint, board);

            context.SaveChanges();
            app.Logger.LogInformation("Database seeding completed successfully");
        }
        catch (Exception exception)
        {
            app.Logger.LogError(exception, "An error occurred while seeding the database");
            throw;
        }
    }

    private static async Task<List<User>> SeedUsers(
        ApplicationDbContext context,
        Organization organization,
        Project project)
    {
        var seedUserData = new[]
        {
            new { FirstName = "John", LastName = "Doe", Email = "john.doe@mirai.com", IdentityId = "user-1-12345-67890-abcdef" },
            new { FirstName = "Jane", LastName = "Smith", Email = "jane.smith@mirai.com", IdentityId = "user-2-23456-78901-bcdefg" },
            new { FirstName = "Bob", LastName = "Johnson", Email = "bob.johnson@mirai.com", IdentityId = "user-3-34567-89012-cdefgh" },
        };

        var users = new List<User>();

        foreach (var userData in seedUserData)
        {
            var user = new User(userData.FirstName, userData.LastName, userData.Email);
            user.SetIdentityId(userData.IdentityId);
            users.Add(user);
            organization.AddUser(user);
            project.AddUser(user);
        }

        await context.AddRangeAsync(users);

        return users;
    }

    private static async Task SeedWikiPages(
        Faker faker,
        ApplicationDbContext context,
        Project project,
        List<User> users,
        int count = 5,
        int maxSubPagesPerWiki = 2)
    {
        var wikiPages = new List<WikiPage>();

        for (int i = 0; i < count; i++)
        {
            var randomUser = faker.PickRandom(users);

            var wikiPage = new WikiPage(
                project.Id,
                faker.Lorem.Sentence(),
                faker.Lorem.Paragraph(),
                randomUser.Id);

            await context.WikiPages.AddAsync(wikiPage);
            wikiPages.Add(wikiPage);
        }

        foreach (var mainPage in wikiPages)
        {
            int subPagesCount = faker.Random.Int(0, maxSubPagesPerWiki);

            for (int j = 0; j < subPagesCount; j++)
            {
                var randomUser = faker.PickRandom(users);

                var subWikiPage = new WikiPage(
                    project.Id,
                    faker.Lorem.Sentence(),
                    faker.Lorem.Paragraph(),
                    randomUser.Id,
                    mainPage.Id);

                await context.WikiPages.AddAsync(subWikiPage);
            }
        }
    }

    private static async Task SeedWorkItems(
        Faker faker,
        ApplicationDbContext context,
        Project project,
        Team team,
        Sprint sprint,
        Board board,
        int featureCount = 5,
        int storiesPerFeature = 3)
    {
        var workItemCode = 1;

        var columns = board.Columns.ToList();
        var columnCount = columns.Count;

        var columnPositions = new Dictionary<Guid, int>();
        foreach (var column in columns)
        {
            columnPositions[column.Id] = 0;
        }

        for (int i = 0; i < featureCount; i++)
        {
            var feature = new WorkItem(
                project.Id,
                workItemCode++,
                faker.Lorem.Sentence(),
                WorkItemType.Feature,
                team.Id,
                sprint.Id);

            feature.Update(description: faker.Lorem.Paragraph());

            await context.WorkItems.AddAsync(feature);

            var featureColumnId = columns[i % columnCount].Id;
            var featureCard = new BoardCard(
                featureColumnId,
                feature.Id,
                columnPositions[featureColumnId]++);
            await context.BoardCards.AddAsync(featureCard);

            for (int j = 0; j < storiesPerFeature; j++)
            {
                var userStory = new WorkItem(
                    project.Id,
                    workItemCode++,
                    faker.Lorem.Sentence(),
                    WorkItemType.UserStory,
                    team.Id,
                    sprint.Id,
                    feature.Id);

                userStory.Update(description: faker.Lorem.Paragraph());

                await context.WorkItems.AddAsync(userStory);

                var userStoryColumnId = columns[((i * storiesPerFeature) + j) % columnCount].Id;
                var userStoryCard = new BoardCard(
                    userStoryColumnId,
                    userStory.Id,
                    columnPositions[userStoryColumnId]++);
                await context.BoardCards.AddAsync(userStoryCard);
            }
        }
    }
}