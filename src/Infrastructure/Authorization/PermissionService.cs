using Application.Abstractions;
using Application.Abstractions.Authorization;
using Domain.Authorization;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Authorization;

internal sealed class PermissionService : IPermissionService
{
    private readonly IApplicationDbContext _context;

    public PermissionService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> HasPermissionAsync(
        Guid userId,
        Permission permission,
        ResourceType resourceType,
        Guid resourceId,
        CancellationToken cancellationToken = default)
    {
        var permissions = await GetEffectivePermissionsAsync(
            userId,
            resourceType,
            resourceId,
            cancellationToken);
        return permissions.Contains(permission);
    }

    public async Task<IReadOnlyCollection<Permission>> GetEffectivePermissionsAsync(
        Guid userId,
        ResourceType resourceType,
        Guid resourceId,
        CancellationToken cancellationToken = default)
    {
        var scope = await ResolveScopeAsync(
            resourceType,
            resourceId,
            cancellationToken);
        if (scope is null)
        {
            return [];
        }

        var (organizationId, projectId, teamId) = scope.Value;

        var permissions = new HashSet<Permission>(
            await GetOrganizationPermissionsAsync(userId, organizationId, cancellationToken));

        if (projectId is not null)
        {
            permissions.UnionWith(await GetProjectPermissionsAsync(
                userId,
                projectId.Value,
                cancellationToken));
        }

        if (teamId is not null)
        {
            permissions.UnionWith(await GetTeamPermissionsAsync(
                userId,
                teamId.Value,
                cancellationToken));
        }

        return permissions;
    }

    private async Task<(Guid OrganizationId, Guid? ProjectId, Guid? TeamId)?> ResolveScopeAsync(
        ResourceType resourceType,
        Guid resourceId,
        CancellationToken cancellationToken)
    {
        if (resourceType == ResourceType.Organization)
        {
            return (resourceId, null, null);
        }

        var (projectId, teamId) = await ResolveProjectAndTeamIdAsync(
            resourceType,
            resourceId,
            cancellationToken);
        if (projectId is null)
        {
            return null;
        }

        var organizationId = await FirstOrDefaultAsync(
            _context.Projects.Where(p => p.Id == projectId).Select(p => p.OrganizationId),
            cancellationToken);

        return organizationId is null ? null : (organizationId.Value, projectId, teamId);
    }

    private Task<(Guid? ProjectId, Guid? TeamId)> ResolveProjectAndTeamIdAsync(
        ResourceType resourceType,
        Guid resourceId,
        CancellationToken cancellationToken)
    {
        return resourceType switch
        {
            ResourceType.Project or ResourceType.Backlog or ResourceType.Dashboard =>
                Task.FromResult<(Guid?, Guid?)>((resourceId, null)),
            ResourceType.Team => ResolveTeamScopeAsync(resourceId, cancellationToken),
            ResourceType.WorkItem => ResolveDirectProjectScopeAsync(
                _context.WorkItems.Where(w => w.Id == resourceId).Select(w => w.ProjectId), cancellationToken),
            ResourceType.WikiPage => ResolveDirectProjectScopeAsync(
                _context.WikiPages.Where(w => w.Id == resourceId).Select(w => w.ProjectId), cancellationToken),
            ResourceType.Tag => ResolveDirectProjectScopeAsync(
                _context.Tags.Where(t => t.Id == resourceId).Select(t => t.ProjectId), cancellationToken),
            ResourceType.Persona => ResolveDirectProjectScopeAsync(
                _context.Personas.Where(p => p.Id == resourceId).Select(p => p.ProjectId), cancellationToken),
            ResourceType.Board => ResolveViaTeamAsync(
                _context.Boards.Where(b => b.Id == resourceId).Select(b => b.TeamId), cancellationToken),
            ResourceType.Sprint => ResolveViaTeamAsync(
                _context.Sprints.Where(s => s.Id == resourceId).Select(s => s.TeamId), cancellationToken),
            ResourceType.Retrospective => ResolveViaTeamAsync(
                _context.Retrospectives.Where(r => r.Id == resourceId).Select(r => r.TeamId), cancellationToken),
            _ => Task.FromResult<(Guid?, Guid?)>((null, null)),
        };
    }

    private async Task<(Guid? ProjectId, Guid? TeamId)> ResolveTeamScopeAsync(
        Guid teamId,
        CancellationToken cancellationToken)
    {
        var projectId = await FirstOrDefaultAsync(
            _context.Teams.Where(t => t.Id == teamId).Select(t => t.ProjectId),
            cancellationToken);

        return (projectId, teamId);
    }

    private async Task<(Guid? ProjectId, Guid? TeamId)> ResolveDirectProjectScopeAsync(
        IQueryable<Guid> projectIdQuery,
        CancellationToken cancellationToken)
    {
        var projectId = await FirstOrDefaultAsync(projectIdQuery, cancellationToken);
        return (projectId, null);
    }

    private async Task<(Guid? ProjectId, Guid? TeamId)> ResolveViaTeamAsync(
        IQueryable<Guid> teamIdQuery,
        CancellationToken cancellationToken)
    {
        var teamId = await FirstOrDefaultAsync(teamIdQuery, cancellationToken);
        return teamId is null ? (null, null) : await ResolveTeamScopeAsync(teamId.Value, cancellationToken);
    }

    private Task<List<Permission>> GetOrganizationPermissionsAsync(
        Guid userId,
        Guid organizationId,
        CancellationToken cancellationToken)
    {
        return _context.Organizations
            .AsNoTracking()
            .Where(o => o.Id == organizationId)
            .SelectMany(o => o.Members)
            .Where(m => m.UserId == userId)
            .SelectMany(m => m.Role.Permissions)
            .Select(p => p.Permission)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    private Task<List<Permission>> GetProjectPermissionsAsync(
        Guid userId,
        Guid projectId,
        CancellationToken cancellationToken)
    {
        return _context.Projects
            .AsNoTracking()
            .Where(p => p.Id == projectId)
            .SelectMany(p => p.Members)
            .Where(m => m.UserId == userId)
            .SelectMany(m => m.Role.Permissions)
            .Select(p => p.Permission)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    private Task<List<Permission>> GetTeamPermissionsAsync(
        Guid userId,
        Guid teamId,
        CancellationToken cancellationToken)
    {
        return _context.Teams
            .AsNoTracking()
            .Where(t => t.Id == teamId)
            .SelectMany(t => t.Members)
            .Where(m => m.UserId == userId)
            .SelectMany(m => m.Role.Permissions)
            .Select(p => p.Permission)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    private static async Task<Guid?> FirstOrDefaultAsync(IQueryable<Guid> query, CancellationToken cancellationToken)
    {
        var results = await query.Take(1).ToListAsync(cancellationToken);
        return results.Count > 0 ? results[0] : null;
    }
}
