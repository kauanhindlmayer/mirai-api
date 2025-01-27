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

namespace WebApi.Extensions;

public static class SeedDataExtensions
{
    public static void SeedData(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Seeding data...");

        if (context.Organizations.Any())
        {
            return;
        }

        var faker = new Faker();

        var organization = new Organization(
            faker.Company.CompanyName(),
            faker.Company.CatchPhrase());
        context.Organizations.Add(organization);

        var project = new Project(
            faker.Commerce.ProductName(),
            faker.Commerce.ProductDescription(),
            organization.Id);
        context.Projects.Add(project);

        var team = new Team(
            project.Id,
            project.Name,
            faker.Lorem.Sentence());
        context.Teams.Add(team);

        var startDate = faker.Date.Recent().ToUniversalTime();
        var sprint = new Sprint(
            team.Id,
            "Sprint 1",
            startDate,
            startDate.AddDays(14));
        context.Sprints.Add(sprint);

        var board = new Board(
            team.Id,
            team.Name);
        context.Boards.Add(board);

        var users = SeedUsers(faker, context);
        SeedWikiPages(faker, context, project, users);
        SeedWorkItems(faker, context, project, team, sprint, board);

        context.SaveChanges();
        logger.LogInformation("Data seeded");
    }

    private static List<User> SeedUsers(
        Faker faker,
        ApplicationDbContext context,
        int count = 5)
    {
        var users = new List<User>();

        for (int i = 0; i < count; i++)
        {
            var user = new User(
                faker.Name.FirstName(),
                faker.Name.LastName(),
                faker.Internet.Email());
            user.SetIdentityId(Guid.NewGuid().ToString());
            users.Add(user);
        }

        context.AddRange(users);
        return users;
    }

    private static void SeedWikiPages(
        Faker faker,
        ApplicationDbContext context,
        Project project,
        List<User> users,
        int count = 5,
        int maxSubPagesPerWiki = 2)
    {
        var wikiPages = new List<WikiPage>();
        var random = new Random();

        for (int i = 0; i < count; i++)
        {
            var randomUser = users[random.Next(users.Count)];

            var wikiPage = new WikiPage(
                project.Id,
                faker.Lorem.Sentence(),
                faker.Lorem.Paragraph(),
                randomUser.Id);

            context.WikiPages.Add(wikiPage);
            wikiPages.Add(wikiPage);
        }

        foreach (var mainPage in wikiPages)
        {
            int subPagesCount = random.Next(0, maxSubPagesPerWiki + 1);

            for (int j = 0; j < subPagesCount; j++)
            {
                var randomUser = users[random.Next(users.Count)];

                var subWikiPage = new WikiPage(
                    project.Id,
                    faker.Lorem.Sentence(),
                    faker.Lorem.Paragraph(),
                    randomUser.Id,
                    mainPage.Id);

                context.WikiPages.Add(subWikiPage);
            }
        }
    }

    private static void SeedWorkItems(
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

            context.WorkItems.Add(feature);

            var featureColumnId = columns[i % columnCount].Id;
            var featureCard = new BoardCard(
                featureColumnId,
                feature.Id,
                columnPositions[featureColumnId]++);
            context.BoardCards.Add(featureCard);

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

                context.WorkItems.Add(userStory);

                var userStoryColumnId = columns[((i * storiesPerFeature) + j) % columnCount].Id;
                var userStoryCard = new BoardCard(
                    userStoryColumnId,
                    userStory.Id,
                    columnPositions[userStoryColumnId]++);
                context.BoardCards.Add(userStoryCard);
            }
        }
    }
}
