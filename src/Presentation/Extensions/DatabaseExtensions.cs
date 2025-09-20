using Bogus;
using Domain.Boards;
using Domain.Organizations;
using Domain.Projects;
using Domain.Shared;
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

            var board = new Board(
                team.Id,
                team.Name);
            await context.Boards.AddAsync(board);

            var sprints = await SeedSprints(context, team);
            var users = await SeedUsers(context, organization, project);
            await SeedWikiPages(faker, context, project, users);
            await SeedWorkItems(faker, context, project, team, sprints, board);

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
            new
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@mirai.com",
                IdentityId = "user-1-12345-67890-abcdef",
            },
            new
            {
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@mirai.com",
                IdentityId = "user-2-23456-78901-bcdefg",
            },
            new
            {
                FirstName = "Bob",
                LastName = "Johnson",
                Email = "bob.johnson@mirai.com",
                IdentityId = "user-3-34567-89012-cdefgh",
            },
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

    private static async Task<List<Sprint>> SeedSprints(
        ApplicationDbContext context,
        Team team,
        int sprintCount = 8)
    {
        var sprints = new List<Sprint>();
        var baseDate = DateTime.UtcNow.AddDays(-60);

        for (int i = 0; i < sprintCount; i++)
        {
            var startDate = baseDate.AddDays(i * 14);
            var endDate = startDate.AddDays(14);

            var sprint = new Sprint(
                team.Id,
                $"Sprint {i + 1}",
                DateOnly.FromDateTime(startDate),
                DateOnly.FromDateTime(endDate));

            sprints.Add(sprint);
            await context.Sprints.AddAsync(sprint);
        }

        return sprints;
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
        List<Sprint> sprints,
        Board board,
        int epicCount = 3,
        int featuresPerEpic = 5,
        int storiesPerFeature = 6,
        int bugsCount = 15)
    {
        var workItemCode = 1;
        var columns = board.Columns.ToList();
        var columnPositions = new Dictionary<Guid, int>();

        foreach (var column in columns)
        {
            columnPositions[column.Id] = 0;
        }

        var orderedSprints = sprints.OrderBy(s => s.StartDate).ToList();
        var baseDate = DateTime.UtcNow.AddDays(-20);

        var epics = new List<WorkItem>();
        for (int i = 0; i < epicCount; i++)
        {
            var epic = new WorkItem(
                project.Id,
                workItemCode++,
                faker.Lorem.Sentence(),
                WorkItemType.Epic,
                team.Id,
                null);

            epic.Update(description: faker.Lorem.Paragraphs(2));
            SetWorkItemDates(epic, baseDate.AddDays(i * 10), faker, 0.9);

            await context.WorkItems.AddAsync(epic);
            epics.Add(epic);

            var epicColumnId = GetColumnForStatus(columns, epic.Status).Id;
            var epicCard = new BoardCard(
                epicColumnId,
                epic.Id,
                columnPositions[epicColumnId]++);
            await context.BoardCards.AddAsync(epicCard);
        }

        var features = new List<WorkItem>();
        foreach (var epic in epics)
        {
            for (int i = 0; i < featuresPerEpic; i++)
            {
                var sprintIndex = faker.Random.Int(0, Math.Min(orderedSprints.Count - 1, 3));
                var feature = new WorkItem(
                    project.Id,
                    workItemCode++,
                    faker.Lorem.Sentence(),
                    WorkItemType.Feature,
                    team.Id,
                    orderedSprints[sprintIndex].Id,
                    epic.Id);

                feature.Update(description: faker.Lorem.Paragraph());
                SetWorkItemDates(
                    feature,
                    baseDate.AddDays(((epics.IndexOf(epic) * featuresPerEpic) + i) * 7),
                    faker,
                    0.8);

                await context.WorkItems.AddAsync(feature);
                features.Add(feature);

                var featureColumnId = GetColumnForStatus(columns, feature.Status).Id;
                var featureCard = new BoardCard(
                    featureColumnId,
                    feature.Id,
                    columnPositions[featureColumnId]++);
                await context.BoardCards.AddAsync(featureCard);
            }
        }

        foreach (var feature in features)
        {
            for (int i = 0; i < storiesPerFeature; i++)
            {
                var sprintIndex = faker.Random.Int(0, orderedSprints.Count - 1);
                var userStory = new WorkItem(
                    project.Id,
                    workItemCode++,
                    faker.Lorem.Sentence(),
                    WorkItemType.UserStory,
                    team.Id,
                    orderedSprints[sprintIndex].Id,
                    feature.Id);

                userStory.Update(description: faker.Lorem.Paragraph());
                SetWorkItemDates(
                    userStory,
                    baseDate.AddDays(((features.IndexOf(feature) * storiesPerFeature) + i) * 3),
                    faker,
                    0.75);

                await context.WorkItems.AddAsync(userStory);

                var userStoryColumnId = GetColumnForStatus(columns, userStory.Status).Id;
                var userStoryCard = new BoardCard(
                    userStoryColumnId,
                    userStory.Id,
                    columnPositions[userStoryColumnId]++);
                await context.BoardCards.AddAsync(userStoryCard);
            }
        }

        for (int i = 0; i < bugsCount; i++)
        {
            var workItemType = faker.Random.Bool() ? WorkItemType.Bug : WorkItemType.Defect;
            var sprintIndex = faker.Random.Int(1, orderedSprints.Count - 1);

            var bug = new WorkItem(
                project.Id,
                workItemCode++,
                faker.Lorem.Sentence(),
                workItemType,
                team.Id,
                orderedSprints[sprintIndex].Id);

            bug.Update(description: faker.Lorem.Paragraph());
            SetWorkItemDates(
                bug,
                baseDate.AddDays(60 + (i * 5)),
                faker,
                0.85);

            await context.WorkItems.AddAsync(bug);

            var bugColumnId = GetColumnForStatus(columns, bug.Status).Id;
            var bugCard = new BoardCard(
                bugColumnId,
                bug.Id,
                columnPositions[bugColumnId]++);
            await context.BoardCards.AddAsync(bugCard);
        }

        await SeedRecentWorkItems(
            faker,
            context,
            project,
            team,
            orderedSprints.Last(),
            board,
            columnPositions,
            workItemCode);
    }

    private static async Task SeedRecentWorkItems(
        Faker faker,
        ApplicationDbContext context,
        Project project,
        Team team,
        Sprint currentSprint,
        Board board,
        Dictionary<Guid, int> columnPositions,
        int startingWorkItemCode,
        int recentItemCount = 20)
    {
        var workItemCode = startingWorkItemCode;
        var columns = board.Columns.ToList();
        var recentBaseDate = DateTime.UtcNow.AddDays(-10);

        for (int i = 0; i < recentItemCount; i++)
        {
            var workItemType = faker.PickRandom(new[] { WorkItemType.UserStory, WorkItemType.Bug });

            var recentWorkItem = new WorkItem(
                project.Id,
                workItemCode++,
                faker.Lorem.Sentence(),
                workItemType,
                team.Id,
                currentSprint.Id);

            recentWorkItem.Update(description: faker.Lorem.Paragraph());

            var creationDate = recentBaseDate.AddDays(faker.Random.Double() * 10);
            var createdAtProperty = typeof(AggregateRoot).GetProperty("CreatedAtUtc");
            createdAtProperty?.SetValue(recentWorkItem, creationDate);

            if (faker.Random.Double() < 0.9)
            {
                var completedStatus = faker.PickRandom(new[] { WorkItemStatus.Closed, WorkItemStatus.Resolved });
                recentWorkItem.Update(status: completedStatus);

                var maxDaysBack = Math.Min(7, (DateTime.UtcNow - creationDate).TotalDays - 0.1);
                var completionDate = DateTime.UtcNow.AddDays(-faker.Random.Double() * Math.Max(0.1, maxDaysBack));

                if (completionDate <= creationDate)
                {
                    completionDate = creationDate.AddHours(faker.Random.Double(1, 24));
                }

                var completedAtProperty = typeof(WorkItem).GetProperty("CompletedAtUtc");
                completedAtProperty?.SetValue(recentWorkItem, completionDate);
            }
            else
            {
                recentWorkItem.Update(status: WorkItemStatus.InProgress);
            }

            await context.WorkItems.AddAsync(recentWorkItem);

            var columnId = GetColumnForStatus(columns, recentWorkItem.Status).Id;
            var card = new BoardCard(
                columnId,
                recentWorkItem.Id,
                columnPositions[columnId]++);
            await context.BoardCards.AddAsync(card);
        }
    }

    private static void SetWorkItemDates(
        WorkItem workItem,
        DateTime baseCreationDate,
        Faker faker,
        double completionProbability)
    {
        var createdAtProperty = typeof(AggregateRoot).GetProperty("CreatedAtUtc");
        var creationDate = baseCreationDate.AddHours(faker.Random.Double() * 24);
        createdAtProperty?.SetValue(workItem, creationDate);

        var daysSinceCreation = (DateTime.UtcNow - creationDate).TotalDays;
        var shouldComplete = faker.Random.Double() < completionProbability && daysSinceCreation > 1;

        if (shouldComplete)
        {
            var completedStatuses = new[] { WorkItemStatus.Closed, WorkItemStatus.Resolved };
            var completedStatus = faker.PickRandom(completedStatuses);

            var minCompletionDays = Math.Max(0.1, daysSinceCreation * 0.2);
            var maxCompletionDays = Math.Min(daysSinceCreation * 0.95, 15);
            var completionDate = creationDate.AddDays(faker.Random.Double(minCompletionDays, maxCompletionDays));

            if (completionDate <= creationDate)
            {
                completionDate = creationDate.AddHours(faker.Random.Double(1, 24));
            }

            if (completionDate > DateTime.UtcNow)
            {
                completionDate = DateTime.UtcNow.AddHours(-faker.Random.Double(1, 12));
            }

            workItem.Update(status: completedStatus);

            var completedAtProperty = typeof(WorkItem).GetProperty("CompletedAtUtc");
            completedAtProperty?.SetValue(workItem, completionDate);
        }
        else
        {
            var inProgressStatuses = new[] { WorkItemStatus.New, WorkItemStatus.InProgress, WorkItemStatus.Reopened };
            var activeStatus = daysSinceCreation > 3 ? WorkItemStatus.InProgress : faker.PickRandom(inProgressStatuses);
            workItem.Update(status: activeStatus);
        }
    }

    private static BoardColumn GetColumnForStatus(List<BoardColumn> columns, WorkItemStatus status)
    {
        var orderedColumns = columns.OrderBy(c => c.Position).ToList();

        return status switch
        {
            WorkItemStatus.New => orderedColumns.First(),
            WorkItemStatus.InProgress => orderedColumns.Count > 1
                ? orderedColumns[1]
                : orderedColumns.First(),
            WorkItemStatus.Resolved => orderedColumns.Count > 2
                ? orderedColumns[^2]
                : orderedColumns.Last(),
            WorkItemStatus.Closed => orderedColumns.Last(),
            WorkItemStatus.Reopened => orderedColumns.First(),
            WorkItemStatus.Removed => orderedColumns.Last(),
            _ => orderedColumns.First(),
        };
    }
}