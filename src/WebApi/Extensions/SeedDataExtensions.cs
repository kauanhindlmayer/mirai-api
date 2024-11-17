using Bogus;
using Domain.Organizations;
using Domain.Projects;
using Domain.WorkItems;
using Domain.WorkItems.Enums;
using Infrastructure.Common.Persistence;

namespace WebApi.Extensions;

public static class SeedDataExtensions
{
    public static void SeedData(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Seeding data...");

        if (dbContext.Organizations.Any())
        {
            return;
        }

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
            }
        }

        dbContext.SaveChanges();
        logger.LogInformation("Data seeded");
    }

    private static List<Organization> GenerateOrganizations(int count = 5)
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

    private static List<WorkItem> GenerateWorkItems(Project project, int count = 5)
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
}
