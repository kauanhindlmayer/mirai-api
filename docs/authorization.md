# Authorization

Reference for the RBAC system: how permissions are resolved, and the two things you'll do most often - add a new permission, and gate a new endpoint with one.

Related: [docs/adr/0006-add-role-based-access-control.md](adr/0006-add-role-based-access-control.md).

## Core concepts

- **`Permission`** (`src/Domain/Authorization/Permission.cs`) - a flat enum, one value per resource-action pair (`ProjectManageWorkItems`, `TeamManageMembers`, etc).
- **`RoleScope`** (`src/Domain/Authorization/RoleScope.cs`) - `Organization`, `Project`, or `Team`.
  Every role belongs to exactly one scope.
- **`Role`** (`src/Domain/Authorization/Role.cs`) - a named bundle of permissions at a given scope.
  Roles are fixed and seeded, not user-definable.
- **`SystemRoles`** (`src/Domain/Authorization/SystemRoles.cs`) - the catalog of 8 seeded roles: Organization (Owner, Admin, Member), Project (Admin, Contributor, Viewer), Team (Admin, Member).
- **`ResourceType`** (`src/Domain/Authorization/ResourceType.cs`) - what kind of thing is being acted on (`Project`, `WorkItem`, `WikiPage`, ...).
  Used to resolve which org/project/team a resource belongs to.

Roles inherit downward: an Organization Owner/Admin has every permission on every Project and Team inside that org, even without being directly added as a project or team member.
A Project role only grants access within that project (and its teams).
A Team role only governs team-scoped concerns.

There is no config flag to disable enforcement - it is always on.

## How permission resolution works

`IPermissionService` (`src/Infrastructure/Authorization/PermissionService.cs`) is the single source of truth.
Given a `ResourceType` + resource id, it:

1. Walks the resource up to its owning `(organizationId, projectId?, teamId?)`. Most resource types have a direct FK to their project or team; `ResourceType.Team`/`Board`/`Sprint`/`Retrospective` resolve via their team, everything else resolves directly.
2. Unions the caller's permissions from their Organization role, Project role (if applicable), and Team role (if applicable) into one set.
3. `HasPermissionAsync` checks whether the required permission is in that set.

If the resource doesn't exist, resolution returns no permissions, so the caller is denied - see [Nonexistent resources](#nonexistent-resources-return-403-not-404) below.

## Enforcing a permission on an endpoint

Every mutating command and most queries implement `IAuthorizationRequest<TResponse>` (`src/Application/Abstractions/Authorization/IAuthorizationRequest.cs`):

```csharp
public interface IAuthorizationRequest
{
    Permission RequiredPermission { get; }
    ResourceType ResourceType { get; }
    Guid ResourceId { get; }
}
```

`AuthorizationBehavior`, a MediatR pipeline behavior registered after `ValidationBehavior` and before `QueryCachingBehavior`/`UnitOfWorkBehavior`, checks this automatically for every request that implements it.
There is nothing to wire up per-endpoint beyond implementing the three properties.

### Steps

1. Implement `IAuthorizationRequest<TResponse>` on the command/query, computing the three properties from the request's own fields:

   ```csharp
   public sealed record DeleteOrganizationCommand(Guid OrganizationId)
       : IAuthorizationRequest<ErrorOr<Success>>
   {
       public Permission RequiredPermission => Permission.OrganizationDelete;
       public ResourceType ResourceType => ResourceType.Organization;
       public Guid ResourceId => OrganizationId;
   }
   ```

   When the id _is_ the resource (e.g. `OrganizationId`), this is trivial.
   For a child resource, point `ResourceType` at the child and `ResourceId` at the child's own id - `PermissionService` resolves the owning project/team internally:

   ```csharp
   public sealed record MoveWikiPageCommand(
       Guid ProjectId,
       Guid WikiPageId,
       Guid? TargetParentId,
       int TargetPosition) : IAuthorizationRequest<ErrorOr<Success>>
   {
       public Permission RequiredPermission => Permission.ProjectManageWikiPages;
       public ResourceType ResourceType => ResourceType.WikiPage;
       public Guid ResourceId => WikiPageId;
   }
   ```

2. Do not also hand-roll a membership filter inside the handler (e.g. `p.Members.Any(m => m.UserId == _userContext.UserId)`) as a second layer of protection.
   `AuthorizationBehavior` already covers it, and a handler-level filter checking _direct_ membership will incorrectly reject Organization Owners/Admins who have inherited access without being direct project/team members.
3. Nothing else to register. `ArchitectureTests.Application.AuthorizationTests` fails the build if any `*Command` doesn't implement `IAuthorizationRequest`.
   The only way around it is adding the command's name to that test's `ExemptCommandNames` list, which should only happen for commands with no pre-existing resource to check against (registration, login, password reset, editing your own profile).

Controllers themselves need no changes - `ApiController.Problem(Error)` already maps `AuthorizationErrors.Forbidden` (an `Error.Unauthorized`) to a 403 response.

### Nonexistent resources return 403, not 404

Authorization is checked before the handler runs, so a request for a resource that doesn't exist returns `Forbidden`, not `NotFound` - the caller has no membership in something that isn't there, so they're denied rather than told what's missing.
This is deliberate (see the ADR): never use `Error.NotFound` for an authorization failure.
Keep this in mind when writing tests for a "resource does not exist" case on an authorized endpoint - assert `Forbidden`, not `NotFound`.

## Adding a new permission

1. Add the value to the **end** of the `Permission` enum (`src/Domain/Authorization/Permission.cs`).
   Don't insert in the middle - `Role.CreateDeterministicPermissionId` derives each seeded `RolePermission`'s id by XOR-ing the role id with `(int)permission`, so renumbering existing values churns their ids for no reason on the next migration.
2. Add it to whichever role(s) in `SystemRoles.cs` should grant it - either to one of the `AllOrganizationPermissions`/`AllProjectAndTeamPermissions` arrays (if every role of that kind should have it) or directly to a specific role's permission list.
3. Generate a migration - EF diffs the `HasData` seed automatically:

   ```bash
   dotnet ef migrations add <MigrationName> --project src/Infrastructure --startup-project src/Presentation -o Persistence/Migrations
   ```

   Verify there's no drift before committing: `dotnet ef migrations has-pending-model-changes --project src/Infrastructure --startup-project src/Presentation` should report no changes.

4. Use the new permission as `RequiredPermission` on whichever command/query needs it (see above).

## Adding a new gateable resource type

If the new permission protects a resource type `PermissionService` doesn't know how to resolve yet (i.e. not already in `ResourceType`), add a case to `ResolveProjectAndTeamIdAsync` in `PermissionService.cs` that maps the new `ResourceType` to its owning project id (directly, or via `ResolveViaTeamAsync` if it only has a team FK).

## Frontend gating (`mirai-react-app`)

- `GET /api/{organizations,projects,teams}/{id}/effective-permissions` returns the caller's permission names for that resource, unioning inherited org-level grants.
  `useOrganizationEffectivePermissionsQuery`/`useProjectEffectivePermissionsQuery`/`useTeamEffectivePermissionsQuery` (`src/queries/roles.ts`) wrap it.
- `useCan(scope, resourceId, permission)` (`src/hooks/use-can.ts`) is the gate: fails closed, returning `false` while loading or when the resource id is unknown - never default to "allowed".
  Use it to hide admin controls (nav items, role selects, remove-member buttons), not as the actual security boundary - the backend is that.
- `GET /api/roles?scope=` (`useRolesQuery`) returns the fixed role catalog for populating role `<Select>`s.

## Testing

- **Integration tests** (`Application.IntegrationTests`) go through the real pipeline, so the caller matters.
  Use `BaseIntegrationTest.SetCurrentUser(userId)` to set who's calling before sending a command/query, and `GetRoleAsync(SystemRoles.XId)` to fetch a role tracked by the test's own `DbContext` (passing a `SystemRoles.X` static instance directly makes EF treat it as a new, untracked entity).
  If a test creates a `Project` directly via `_dbContext.Projects.Add(...)` rather than through `CreateProjectCommand`, call `SeedCurrentUserAsync()` first - creating a project raises a domain event that creates a default wiki page authored by the current user, which needs a real, already-persisted user to reference.
- **Functional tests** (`Presentation.FunctionalTests`) authenticate as two seeded users via `DistributedApplicationTestFixture`: `HttpClient` (John Doe, Owner/Admin everywhere) and `SecondaryHttpClient` (Jane Smith, Member/Contributor everywhere).
  Use the latter to assert an endpoint actually returns 403 for someone without the required permission, not just 200 for someone who has it.
