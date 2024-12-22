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

                var wikiPages = GenerateWikiPages(project, users.First());
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
            .RuleFor(p => p.OrganizationId, f => organization.Id);

        return projectFaker.Generate(count);
    }

    private static List<WorkItem> GenerateWorkItems(Project project, int count = 250)
    {
        var workItemCode = 1;

        var workItemFaker = new Faker<WorkItem>()
            .RuleFor(wi => wi.Code, f => workItemCode++)
            .RuleFor(wi => wi.Title, f => f.Random.Words(3))
            .RuleFor(wi => wi.Description, f => f.Lorem.Paragraph())
            .RuleFor(wi => wi.AcceptanceCriteria, f => f.Lorem.Paragraph())
            .RuleFor(wi => wi.ProjectId, f => project.Id)
            .RuleFor(wi => wi.Type, f => f.PickRandom<WorkItemType>(WorkItemType.List))
            .RuleFor(wi => wi.Status, f => f.PickRandom<WorkItemStatus>(WorkItemStatus.List));

        return workItemFaker.Generate(count);
    }

    private static List<WikiPage> GenerateWikiPages(
        Project project,
        User author,
        int count = 10,
        bool isSubPage = false)
    {
        var position = 1;
        var subPages = !isSubPage ? GenerateWikiPages(project, author, 3, true) : [];

        var wikiPageFaker = new Faker<WikiPage>()
            .RuleFor(wp => wp.Title, f => f.Lorem.Sentence())
            .RuleFor(wp => wp.Content, f => f.Lorem.Paragraph())
            .RuleFor(wp => wp.Position, f => position++)
            .RuleFor(wp => wp.AuthorId, f => author.Id)
            .RuleFor(wp => wp.ProjectId, f => project.Id)
            .RuleFor(wp => wp.SubWikiPages, f => subPages)
            .RuleFor(wp => wp.CreatedAt, f => DateTime.UtcNow)
            .RuleFor(wp => wp.UpdatedAt, f => DateTime.UtcNow);

        return wikiPageFaker.Generate(count);
    }
}
