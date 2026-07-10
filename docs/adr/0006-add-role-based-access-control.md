# 6. Add role-based access control

Date: 2026-07-09

## Status

Accepted

## Context

Mirai has authentication (Keycloak/JWT) but effectively no authorization.
Every mutating endpoint is gated only by "is the caller logged in," and membership between Users and Organizations/Projects/Teams is a bare many-to-many join with no role information.
Any authenticated user can add or remove members on any organization or project they can guess a GUID for, delete organizations, and so on.

We evaluated the permission models of Jira (fully custom, per-project permission schemes) and Azure DevOps (a small fixed set of default security groups, with custom groups as an advanced option) as reference points.

## Decision

We will implement a fixed-role RBAC system, closer to Azure DevOps' model than Jira's:

- A small, seeded, non-custom set of system roles per scope: Organization (Owner, Admin, Member), Project (Admin, Contributor, Viewer), Team (Admin, Member).
- A fine-grained `Permission` catalog under the hood, bundled coarsely into the roles above, so custom roles remain an easy addition later without a redesign.
- Role assignment inherits down the Organization → Project → Team hierarchy: an Organization Owner/Admin implicitly has full access to every Project/Team inside that org.
- Enforcement via a new MediatR pipeline behavior (`AuthorizationBehavior`) and an `IPermissionService`.
- The existing `organization_users`/`project_users`/`team_users` join tables become first-class `OrganizationMember`/`ProjectMember`/`TeamMember` entities carrying a role.

## Alternatives Considered

- **Jira-style fully custom permission schemes** (user-definable roles with arbitrary permission sets per project) — rejected for this iteration as significantly more domain, UI, and migration complexity than the product currently needs.
  The fine-grained permission catalog keeps this path open later.
- **Bitmask/flags enum for role permissions** instead of a `role_permissions` join table — rejected because it caps the permission count, makes SQL-level queries harder, and is fragile if custom roles are added later.

## Consequences

- Every mutating command/query needs an authorization marker interface (`IAuthorizationRequest`) declaring its required permission and resource scope; an ArchitectureTest enforces this isn't silently skipped.
- The frontend (`mirai-react-app`) needs a parallel set of changes: role-aware member management UI, a `useCan` permission-gating hook, and nav-item gating.
