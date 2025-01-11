using Domain.Boards;
using Domain.Organizations;
using Domain.Projects;
using Domain.Retrospectives;
using Domain.Tags;
using Domain.Teams;
using Domain.Users;
using Domain.WikiPages;
using Domain.WorkItems;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces.Persistence;

public interface IApplicationDbContext
{
    DbSet<Board> Boards { get; }
    DbSet<Organization> Organizations { get; }
    DbSet<Project> Projects { get; }
    DbSet<Retrospective> Retrospectives { get; }
    DbSet<Tag> Tags { get; }
    DbSet<Team> Teams { get; }
    DbSet<User> Users { get; }
    DbSet<WikiPage> WikiPages { get; }
    DbSet<WorkItem> WorkItems { get; }
}