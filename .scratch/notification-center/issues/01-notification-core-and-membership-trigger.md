# 01 — Notification core + membership trigger

**What to build:** The foundational Notification Center: a project member who is added to a Project, Team, or Organization sees it as a notification. They can open a bell/panel showing their notifications newest-first, click one to navigate to what it's about and mark it read, or mark everything read in one action. This ticket also establishes the "stage a Notification, raise a lightweight domain event" creation pattern that every later trigger (assigned work item changes, comments, mentions) will reuse without re-deriving it - and the "notification created" domain event is raised here even though nothing consumes it for real-time push yet (that's ticket 02).

**Blocked by:** None — can start immediately.

**Status:** ready-for-agent

- [ ] A `Notification` entity exists (`RecipientUserId`, `NotificationType`, `EntityId`, a `Message` string resolved and stored at creation time - not re-derived at read time, `ReadAtUtc` nullable), with `NotificationType` values at least covering `AddedToProject`, `AddedToTeam`, `AddedToOrganization` (room for the other types tickets 03/04 will add).
- [ ] `Notification` cascade-deletes with its source entity (e.g. the Project/Team/Organization) when that's deleted.
- [ ] New notification handlers on the existing `UserAddedToProjectDomainEvent`, `UserAddedToTeamDomainEvent`, and `UserAddedToOrganizationDomainEvent` create the corresponding notification for the added user - added alongside the existing email handlers (Project, Organization), not replacing them. `UserAddedToTeamDomainEvent` currently has no handler at all, so this is its first consumer.
- [ ] The user who performed the add is never notified about their own action if they add themselves (self-suppression, matching the rest of the feature's convention even though this trigger rarely hits it).
- [ ] A paginated query returns the current user's notifications newest-first (mirroring the existing `GetWorkItemHistoryQuery`/`PaginatedList` convention), with an unread-only filter option.
- [ ] A command marks a single notification read (and is a no-op or safely rejects marking another user's notification).
- [ ] A command marks all of the current user's notifications read in one action.
- [ ] REST endpoints expose: paginated list (with unread-only filter), mark-one-read, mark-all-read - as a top-level `notifications` resource scoped to the authenticated user, not nested under a project/organization.
- [ ] Frontend: a bell icon with an unread-count badge in the main navigation, opening a panel listing notifications (fetched via REST, no live push yet - that's ticket 02).
- [ ] Frontend: clicking a notification navigates to its target (the project/team/organization) and marks it read; opening the panel alone does not mark anything read.
- [ ] Frontend: an explicit "mark all as read" control in the panel.
- [ ] Backend integration tests (real commands through `ISender`, following the `BaseIntegrationTest` convention) cover: each of the three membership triggers creating the right notification, self-suppression, and both read-state commands.
- [ ] Frontend component tests (MSW-mocked, following the `renderWithProviders` convention) cover: rendering the list, click-to-navigate-and-mark-read, and mark-all-read.
