using Domain.Boards;
using Domain.Common;
using Domain.Organizations;
using Domain.Personas;
using Domain.Projects.Events;
using Domain.Tags;
using Domain.Teams;
using Domain.WikiPages;
using Domain.WorkItems;
using ErrorOr;

namespace Domain.Projects;

public sealed class Project : AggregateRoot
{
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public Guid OrganizationId { get; private set; }
    public Organization Organization { get; private set; } = null!;
    public ICollection<WorkItem> WorkItems { get; private set; } = [];
    public List<WikiPage> WikiPages { get; private set; } = [];
    public ICollection<Team> Teams { get; private set; } = [];
    public ICollection<Tag> Tags { get; private set; } = [];
    public ICollection<Persona> Personas { get; private set; } = [];

    public Project(string name, string description, Guid organizationId)
    {
        Name = name;
        Description = description;
        OrganizationId = organizationId;
        AddDomainEvent(new ProjectCreatedDomainEvent(this));
    }

    private Project()
    {
    }

    public void Update(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public ErrorOr<Success> AddWorkItem(WorkItem workItem)
    {
        var team = Teams.FirstOrDefault(t => t.Id == workItem.AssignedTeamId);
        if (team is null)
        {
            return TeamErrors.NotFound;
        }

        var card = new BoardCard(team.Board.DefaultColumn.Id, workItem.Id);
        var result = team.Board.AddCard(card);
        if (result.IsError)
        {
            return result.Errors;
        }

        WorkItems.Add(workItem);
        return Result.Success;
    }

    public ErrorOr<Success> AddWikiPage(WikiPage wikiPage)
    {
        if (WikiPages.Any(wp => wp.Title == wikiPage.Title))
        {
            return ProjectErrors.WikiPageWithSameTitleAlreadyExists;
        }

        if (wikiPage.ParentWikiPageId is not null)
        {
            var parent = WikiPages.FirstOrDefault(wp => wp.Id == wikiPage.ParentWikiPageId);
            if (parent is null)
            {
                return WikiPageErrors.ParentWikiPageNotFound;
            }

            return parent.InsertSubWikiPage(parent.SubWikiPages.Count, wikiPage);
        }

        wikiPage.UpdatePosition(WikiPages.Count);
        WikiPages.Add(wikiPage);
        return Result.Success;
    }

    public ErrorOr<Success> MoveWikiPage(Guid wikiPageId, Guid? targetParentId, int targetPosition)
    {
        var wikiPage = WikiPages.FirstOrDefault(wp => wp.Id == wikiPageId);
        if (wikiPage is null)
        {
            return WikiPageErrors.NotFound;
        }

        WikiPages.Remove(wikiPage);

        if (targetParentId is not null)
        {
            var targetParent = WikiPages.FirstOrDefault(wp => wp.Id == targetParentId);
            if (targetParent is null)
            {
                return WikiPageErrors.ParentWikiPageNotFound;
            }

            targetParent.InsertSubWikiPage(targetPosition, wikiPage);
        }
        else
        {
            if (targetPosition < 0 || targetPosition > WikiPages.Count)
            {
                return WikiPageErrors.InvalidPosition;
            }

            wikiPage.RemoveParent();
            ShiftWikiPages(targetPosition, 1);
            wikiPage.UpdatePosition(targetPosition);
            WikiPages.Add(wikiPage);
        }

        return Result.Success;
    }

    public ErrorOr<Success> AddTeam(Team team)
    {
        if (Teams.Any(t => t.Name == team.Name))
        {
            return ProjectErrors.TeamWithSameNameAlreadyExists;
        }

        Teams.Add(team);
        return Result.Success;
    }

    public ErrorOr<Success> AddTag(Tag tag)
    {
        if (Tags.Any(t => t.Name == tag.Name))
        {
            return TagErrors.AlreadyExists;
        }

        Tags.Add(tag);
        return Result.Success;
    }

    public ErrorOr<Success> RemoveTag(Tag tag)
    {
        if (tag.WorkItems.Count > 0)
        {
            return TagErrors.TagHasWorkItems;
        }

        Tags.Remove(tag);
        return Result.Success;
    }

    public ErrorOr<Success> AddPersona(Persona persona)
    {
        if (Personas.Any(p => p.Name == persona.Name))
        {
            return PersonaErrors.AlreadyExists;
        }

        Personas.Add(persona);
        return Result.Success;
    }

    public ErrorOr<Success> RemovePersona(Guid personaId)
    {
        var persona = Personas.FirstOrDefault(p => p.Id == personaId);
        if (persona is null)
        {
            return PersonaErrors.NotFound;
        }

        Personas.Remove(persona);
        return Result.Success;
    }

    private void ShiftWikiPages(int fromIndex, int offset)
    {
        foreach (var page in WikiPages.Where(p => p.Position >= fromIndex))
        {
            page.UpdatePosition(page.Position + offset);
        }
    }
}