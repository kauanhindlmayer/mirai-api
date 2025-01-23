using Bogus;
using Domain.Organizations;
using Domain.Projects;
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
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Seeding data...");

        if (dbContext.Organizations.Any())
        {
            return;
        }

        var users = GenerateUsers();
        dbContext.Users.AddRange(users);

        var organizations = GenerateOrganizations();
        dbContext.Organizations.AddRange(organizations);

        foreach (var organization in organizations)
        {
            var projects = GenerateProjects(organization);
            dbContext.Projects.AddRange(projects);

            foreach (var project in projects)
            {
                var workItems = GenerateWorkItems(project);
                dbContext.WorkItems.AddRange(workItems);

                var wikiPages = GenerateWikiPages(project, users);
                dbContext.WikiPages.AddRange(wikiPages);
            }
        }

        dbContext.SaveChanges();
        logger.LogInformation("Data seeded");
    }

    private static List<User> GenerateUsers(int count = 10)
    {
        var userFaker = new Faker<User>()
            .RuleFor(u => u.IdentityId, f => f.Random.Guid().ToString())
            .RuleFor(u => u.FirstName, f => f.Name.FirstName())
            .RuleFor(u => u.LastName, f => f.Name.LastName())
            .RuleFor(u => u.Email, f => f.Internet.Email());

        return userFaker.Generate(count);
    }

    private static List<Organization> GenerateOrganizations(int count = 1)
    {
        var organizationFaker = new Faker<Organization>()
            .RuleFor(o => o.Name, f => f.Company.CompanyName())
            .RuleFor(o => o.Description, f => f.Company.CatchPhrase());

        return organizationFaker.Generate(count);
    }

    private static List<Project> GenerateProjects(Organization organization, int count = 5)
    {
        var projectFaker = new Faker<Project>()
            .RuleFor(p => p.Name, f => f.Commerce.ProductName())
            .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
            .RuleFor(p => p.OrganizationId, _ => organization.Id);

        return projectFaker.Generate(count);
    }

    private static List<WorkItem> GenerateWorkItems(Project project, int count = 250)
    {
        var workItemCode = 1;

        var workItemFaker = new Faker<WorkItem>()
            .RuleFor(wi => wi.Code, _ => workItemCode++)
            .RuleFor(wi => wi.Title, f => f.Random.Words(3))
            .RuleFor(wi => wi.Description, f => f.Lorem.Paragraph())
            .RuleFor(wi => wi.AcceptanceCriteria, f => f.Lorem.Paragraph())
            .RuleFor(wi => wi.ProjectId, _ => project.Id)
            .RuleFor(wi => wi.Type, f => f.PickRandom(Enum.GetValues<WorkItemType>()))
            .RuleFor(wi => wi.Status, f => f.PickRandom(Enum.GetValues<WorkItemStatus>()));

        return workItemFaker.Generate(count);
    }

    private static List<WikiPage> GenerateWikiPages(
        Project project,
        List<User> users,
        int count = 10,
        int maxDepth = 2,
        int currentDepth = 0)
    {
        var position = 1;
        var random = new Random();
        var author = users[random.Next(users.Count)];

        var wikiPageFaker = new Faker<WikiPage>()
            .RuleFor(wp => wp.Title, f => f.Lorem.Sentence())
            .RuleFor(wp => wp.Content, f => f.Lorem.Paragraph())
            .RuleFor(wp => wp.Position, _ => position++)
            .RuleFor(wp => wp.AuthorId, _ => author.Id)
            .RuleFor(wp => wp.ProjectId, _ => project.Id)
            .RuleFor(wp => wp.SubWikiPages, _ => GenerateSubWikiPages(
                project,
                users,
                random.Next(1, 4),
                maxDepth,
                currentDepth + 1))
            .RuleFor(wp => wp.CreatedAt, _ => DateTime.UtcNow)
            .RuleFor(wp => wp.UpdatedAt, _ => DateTime.UtcNow);

        return wikiPageFaker.Generate(count);
    }

    private static List<WikiPage> GenerateSubWikiPages(
        Project project,
        List<User> users,
        int count = 5,
        int maxDepth = 2,
        int currentDepth = 0)
    {
        var random = new Random();
        var shouldGenerateSubPages = currentDepth < maxDepth && random.NextDouble() < 0.5;
        var subWikiPages = shouldGenerateSubPages
            ? GenerateWikiPages(project, users, count, maxDepth, currentDepth + 1)
            : [];
        return subWikiPages;
    }
}
