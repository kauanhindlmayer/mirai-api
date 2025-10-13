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
            var users = await SeedUsers(context, organization, project, team);
            await SeedWikiPages(faker, context, project, users);
            await SeedWorkItems(faker, context, project, team, sprints, board, users);

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
        Project project,
        Team team)
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
            new
            {
                FirstName = "Alice",
                LastName = "Anderson",
                Email = "alice.anderson@mirai.com",
                IdentityId = "user-4-45678-90123-defghi",
            },
            new
            {
                FirstName = "Michael",
                LastName = "Brown",
                Email = "michael.brown@mirai.com",
                IdentityId = "user-5-56789-01234-efghij",
            },
            new
            {
                FirstName = "Sarah",
                LastName = "Wilson",
                Email = "sarah.wilson@mirai.com",
                IdentityId = "user-6-67890-12345-fghijk",
            },
            new
            {
                FirstName = "David",
                LastName = "Miller",
                Email = "david.miller@mirai.com",
                IdentityId = "user-7-78901-23456-ghijkl",
            },
            new
            {
                FirstName = "Emma",
                LastName = "Davis",
                Email = "emma.davis@mirai.com",
                IdentityId = "user-8-89012-34567-hijklm",
            },
            new
            {
                FirstName = "James",
                LastName = "Garcia",
                Email = "james.garcia@mirai.com",
                IdentityId = "user-9-90123-45678-ijklmn",
            },
            new
            {
                FirstName = "Olivia",
                LastName = "Martinez",
                Email = "olivia.martinez@mirai.com",
                IdentityId = "user-10-01234-56789-jklmno",
            },
            new
            {
                FirstName = "William",
                LastName = "Rodriguez",
                Email = "william.rodriguez@mirai.com",
                IdentityId = "user-11-12345-67890-klmnop",
            },
            new
            {
                FirstName = "Sophia",
                LastName = "Hernandez",
                Email = "sophia.hernandez@mirai.com",
                IdentityId = "user-12-23456-78901-lmnopq",
            },
            new
            {
                FirstName = "Benjamin",
                LastName = "Lopez",
                Email = "benjamin.lopez@mirai.com",
                IdentityId = "user-13-34567-89012-mnopqr",
            },
            new
            {
                FirstName = "Isabella",
                LastName = "Gonzalez",
                Email = "isabella.gonzalez@mirai.com",
                IdentityId = "user-14-45678-90123-nopqrs",
            },
            new
            {
                FirstName = "Lucas",
                LastName = "Lee",
                Email = "lucas.lee@mirai.com",
                IdentityId = "user-15-56789-01234-opqrst",
            },
            new
            {
                FirstName = "Mia",
                LastName = "Taylor",
                Email = "mia.taylor@mirai.com",
                IdentityId = "user-16-67890-12345-pqrstu",
            },
            new
            {
                FirstName = "Henry",
                LastName = "Thomas",
                Email = "henry.thomas@mirai.com",
                IdentityId = "user-17-78901-23456-qrstuv",
            },
            new
            {
                FirstName = "Charlotte",
                LastName = "Jackson",
                Email = "charlotte.jackson@mirai.com",
                IdentityId = "user-18-89012-34567-rstuvw",
            },
            new
            {
                FirstName = "Alexander",
                LastName = "White",
                Email = "alexander.white@mirai.com",
                IdentityId = "user-19-90123-45678-stuvwx",
            },
            new
            {
                FirstName = "Amelia",
                LastName = "Harris",
                Email = "amelia.harris@mirai.com",
                IdentityId = "user-20-01234-56789-tuvwxy",
            },
            new
            {
                FirstName = "Daniel",
                LastName = "Clark",
                Email = "daniel.clark@mirai.com",
                IdentityId = "user-21-12345-67890-uvwxyz",
            },
            new
            {
                FirstName = "Harper",
                LastName = "Lewis",
                Email = "harper.lewis@mirai.com",
                IdentityId = "user-22-23456-78901-vwxyza",
            },
            new
            {
                FirstName = "Matthew",
                LastName = "Robinson",
                Email = "matthew.robinson@mirai.com",
                IdentityId = "user-23-34567-89012-wxyzab",
            },
            new
            {
                FirstName = "Evelyn",
                LastName = "Walker",
                Email = "evelyn.walker@mirai.com",
                IdentityId = "user-24-45678-90123-xyzabc",
            },
            new
            {
                FirstName = "Anthony",
                LastName = "Hall",
                Email = "anthony.hall@mirai.com",
                IdentityId = "user-25-56789-01234-yzabcd",
            },
            new
            {
                FirstName = "Abigail",
                LastName = "Allen",
                Email = "abigail.allen@mirai.com",
                IdentityId = "user-26-67890-12345-zabcde",
            },
            new
            {
                FirstName = "Christopher",
                LastName = "Young",
                Email = "christopher.young@mirai.com",
                IdentityId = "user-27-78901-23456-abcdef",
            },
            new
            {
                FirstName = "Emily",
                LastName = "King",
                Email = "emily.king@mirai.com",
                IdentityId = "user-28-89012-34567-bcdefg",
            },
            new
            {
                FirstName = "Joshua",
                LastName = "Wright",
                Email = "joshua.wright@mirai.com",
                IdentityId = "user-29-90123-45678-cdefgh",
            },
            new
            {
                FirstName = "Elizabeth",
                LastName = "Scott",
                Email = "elizabeth.scott@mirai.com",
                IdentityId = "user-30-01234-56789-defghi",
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
            team.AddUser(user);
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
        List<User> users,
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

            if (faker.Random.Bool(0.7f))
            {
                epic.UpdateAssignment(faker.PickRandom(users).Id);
            }

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

                if (faker.Random.Bool(0.8f))
                {
                    feature.UpdateAssignment(faker.PickRandom(users).Id);
                }

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

                if (faker.Random.Bool(0.9f))
                {
                    userStory.UpdateAssignment(faker.PickRandom(users).Id);
                }

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

            var parentFeatureId = faker.Random.Bool(0.7f) ? faker.PickRandom(features).Id : (Guid?)null;

            var bug = new WorkItem(
                project.Id,
                workItemCode++,
                faker.Lorem.Sentence(),
                workItemType,
                team.Id,
                orderedSprints[sprintIndex].Id,
                parentFeatureId);

            bug.Update(description: faker.Lorem.Paragraph());
            SetWorkItemDates(
                bug,
                baseDate.AddDays(60 + (i * 5)),
                faker,
                0.85);

            if (faker.Random.Bool(0.85f))
            {
                bug.UpdateAssignment(faker.PickRandom(users).Id);
            }

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
            workItemCode,
            features,
            users);
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
        List<WorkItem> features,
        List<User> users,
        int recentItemCount = 20)
    {
        var workItemCode = startingWorkItemCode;
        var columns = board.Columns.ToList();
        var recentBaseDate = DateTime.UtcNow.AddDays(-10);

        for (int i = 0; i < recentItemCount; i++)
        {
            var workItemType = faker.PickRandom(new[] { WorkItemType.UserStory, WorkItemType.Bug });

            var parentFeatureId = faker.Random.Bool(0.8f) ? faker.PickRandom(features).Id : (Guid?)null;

            var recentWorkItem = new WorkItem(
                project.Id,
                workItemCode++,
                faker.Lorem.Sentence(),
                workItemType,
                team.Id,
                currentSprint.Id,
                parentFeatureId);

            recentWorkItem.Update(description: faker.Lorem.Paragraph());

            if (faker.Random.Bool(0.95f))
            {
                recentWorkItem.UpdateAssignment(faker.PickRandom(users).Id);
            }

            var creationDate = recentBaseDate.AddDays(faker.Random.Double() * 10);
            var createdAtProperty = typeof(AggregateRoot).GetProperty("CreatedAtUtc");
            createdAtProperty?.SetValue(recentWorkItem, creationDate);

            if (faker.Random.Double() < 0.9)
            {
                var completedStatus = faker.PickRandom(new[] { WorkItemStatus.Closed, WorkItemStatus.Resolved });

                var maxDaysBack = Math.Min(7, (DateTime.UtcNow - creationDate).TotalDays - 0.1);
                var completionDate = DateTime.UtcNow.AddDays(-faker.Random.Double() * Math.Max(0.1, maxDaysBack));

                if (completionDate <= creationDate)
                {
                    completionDate = creationDate.AddHours(faker.Random.Double(1, 24));
                }

                var startWorkDelay = faker.Random.Double(0.1, 0.4);
                var startedDate = creationDate.AddTicks((long)((completionDate - creationDate).Ticks * startWorkDelay));

                var startedAtProperty = typeof(WorkItem).GetProperty("StartedAtUtc");
                startedAtProperty?.SetValue(recentWorkItem, startedDate);

                recentWorkItem.Update(status: completedStatus);

                var completedAtProperty = typeof(WorkItem).GetProperty("CompletedAtUtc");
                completedAtProperty?.SetValue(recentWorkItem, completionDate);
            }
            else
            {
                var daysSinceCreation = (DateTime.UtcNow - creationDate).TotalDays;
                var startWorkDelay = faker.Random.Double(0.1, 0.8);
                var startedDate = creationDate.AddDays(daysSinceCreation * startWorkDelay);

                if (startedDate > DateTime.UtcNow)
                {
                    startedDate = DateTime.UtcNow.AddHours(-faker.Random.Double(1, 12));
                }

                var startedAtProperty = typeof(WorkItem).GetProperty("StartedAtUtc");
                startedAtProperty?.SetValue(recentWorkItem, startedDate);

                recentWorkItem.Update(status: WorkItemStatus.Active);
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

            var startWorkDelay = faker.Random.Double(0.1, 0.4);
            var startedDate = creationDate.AddTicks((long)((completionDate - creationDate).Ticks * startWorkDelay));

            var startedAtProperty = typeof(WorkItem).GetProperty("StartedAtUtc");
            startedAtProperty?.SetValue(workItem, startedDate);

            workItem.Update(status: completedStatus);

            var completedAtProperty = typeof(WorkItem).GetProperty("CompletedAtUtc");
            completedAtProperty?.SetValue(workItem, completionDate);
        }
        else
        {
            var activeStatuses = new[] { WorkItemStatus.New, WorkItemStatus.Active };
            var activeStatus = daysSinceCreation > 3 ? WorkItemStatus.Active : faker.PickRandom(activeStatuses);

            if (activeStatus == WorkItemStatus.Active)
            {
                var startWorkDelay = faker.Random.Double(0.1, 0.8);
                var startedDate = creationDate.AddDays(daysSinceCreation * startWorkDelay);

                if (startedDate > DateTime.UtcNow)
                {
                    startedDate = DateTime.UtcNow.AddHours(-faker.Random.Double(1, 12));
                }

                var startedAtProperty = typeof(WorkItem).GetProperty("StartedAtUtc");
                startedAtProperty?.SetValue(workItem, startedDate);
            }

            workItem.Update(status: activeStatus);
        }
    }

    private static BoardColumn GetColumnForStatus(List<BoardColumn> columns, WorkItemStatus status)
    {
        var orderedColumns = columns.OrderBy(c => c.Position).ToList();

        return status switch
        {
            WorkItemStatus.New => orderedColumns.First(),
            WorkItemStatus.Active => orderedColumns.Count > 1
                ? orderedColumns[1]
                : orderedColumns.First(),
            WorkItemStatus.Resolved => orderedColumns.Count > 2
                ? orderedColumns[^2]
                : orderedColumns.Last(),
            WorkItemStatus.Closed => orderedColumns.Last(),
            WorkItemStatus.Removed => orderedColumns.Last(),
            _ => orderedColumns.First(),
        };
    }
}