# 8. Notification center

Date: 2026-07-12

## Status

Accepted

## Context

Users have no way to learn about activity relevant to them - being `@mentioned` in a comment (mentions already exist end-to-end in the frontend editor via `MentionableEditor`/TipTap, but nothing server-side consumes them), a change to a work item assigned to them, a new comment on their assigned work item, or being added to a project, team, or organization (the last already sends an email today, via `UserAddedToProjectDomainEventHandler` and its Organization equivalent, but has no in-app counterpart).

`WorkItemChangeSet`/`WorkItemChange` (ADR-0007) already gives us a generic, field-agnostic diff of every work item save, built by an EF Core `SaveChanges` interceptor (`WorkItemChangeHistoryInterceptor`, currently 285 of 300 allowed lines) that runs before `SaveChangesAsync` commits, ahead of `DomainEventPublishingInterceptor` in the pipeline. Comments are deliberately excluded from that changelog (ADR-0007's own decision) and have no domain event raised on creation today.

## Decision

- A new `Notification` entity (`Domain/Notifications/`) stores a recipient, a `NotificationType`, an `EntityId` for click-through navigation, and a **pre-rendered display message resolved at write time** - the same reasoning as `WorkItemChange.OldValue`/`NewValue`: a notification must still read correctly after the actor renames themselves or leaves the project.
- `NotificationType` is one value per concrete surface, not a generic scope enum: `MentionedInWorkItemComment`, `MentionedInWikiPageComment`, `AssignedWorkItemChanged`, `WorkItemCommentAdded`, `AddedToProject`, `AddedToTeam`, `AddedToOrganization`. `EntityId`'s meaning is type-dependent (a `WorkItemId`, a `ProjectId`, etc.).
- Fan-out uses two different mechanisms depending on what already exists, not one uniform mechanism for all four triggers:
  - **A new sibling `SaveChanges` interceptor** (not an extension of `WorkItemChangeHistoryInterceptor`, which is already near its line limit) registered immediately after it. It reads the `WorkItemChangeSet` entries the existing interceptor just staged in the `ChangeTracker` - reusing their already-resolved display strings rather than re-diffing - to notify an assignee once per changeset (not once per field), and reads newly-added `WorkItemComment`/`WikiPageComment` entries to notify the work item's assignee plus any users whose mention node (`data-type="mention" data-id="{userId}"`) appears in the saved HTML.
  - **New `INotificationHandler`s on the existing** `UserAddedToProjectDomainEvent`, `UserAddedToTeamDomainEvent`, `UserAddedToOrganizationDomainEvent` for the membership triggers, sitting alongside the existing email handlers rather than replacing them.
  - We explicitly rejected introducing new domain events for every trigger (e.g. a `WorkItemCommentAddedDomainEvent`, a per-field-change event) as the uniform alternative - that re-opens exactly the "domain event per field" design ADR-0007 rejected for change history, and would recompute the diff/display-string resolution the interceptor already does.
- The actor is never notified of their own action (self-mention, self-triggered change, self-added comment).
- Changes whose `WorkItemChangeSet.ChangedByUserId` is null (background-job/`SystemActor`-originated, e.g. the GitHub sync jobs) do **not** notify the assignee - only human-originated changes do.
- Delivery is in-app only for v1: persisted `Notification` rows plus a real-time push over a new SignalR hub. No email integration for the four new trigger types; the existing "Added to Project/Organization" emails are untouched and unrelated to this feature.
- The new hub pushes the **full rendered `Notification` object** (not a thin refetch signal like `GitHubIntegrationHub`'s pattern) to the specific recipient only, via `Clients.User(...)` and a new `IUserIdProvider` mapping SignalR connections to the same `UserId` the rest of the app reads from `IUserContext` - this is new plumbing, since both existing hubs (`RetrospectiveHub`, `GitHubIntegrationHub`) broadcast to `Clients.All` and have no per-user privacy concern.
- Read state changes only on an explicit action: clicking a notification (which also navigates to its `EntityId`) or an explicit "mark all as read." Opening the notification panel does not mark anything read.
- `NotificationPreference` is a per-user row with one boolean per trigger *category* (Mentions, Assigned Work Item Changes, Work Item Comments, Membership - the three `AddedTo*` types share one flag), defaulting to enabled, checked before a `Notification` row is written.
- No retention/deletion policy, consistent with the rest of the codebase having no soft-delete or audit-retention precedent (ADR-0007). `Notification` cascade-deletes with its source entity when that's deleted, same as `WorkItemChangeSet` does with its `WorkItem`.

## Alternatives Considered

- **Uniform domain-event mechanism for all four triggers** - rejected: duplicates work `WorkItemChangeHistoryInterceptor` already does for the changeset case, and reintroduces the per-field-event pattern ADR-0007 chose against.
- **Extending `WorkItemChangeHistoryInterceptor` directly** instead of adding a sibling - rejected: the class is already at 285/300 lines, and mixes change-history recording with notification fan-out, two different responsibilities.
- **Structured references instead of a pre-rendered message** (recipient/actor/entity IDs only, frontend composes the sentence) - rejected: reintroduces the exact staleness problem (renamed/removed actors) ADR-0007 solved by resolving display strings at write time, and adds N+1-style lookups to render a notification list.
- **Auto-mark-as-read on opening the panel** - rejected: unread items would disappear before the user actually looked at them.
- **Email for every notification** - rejected as substantially larger v1 scope (generic template path across four trigger types, plus throttling/digesting to avoid inbox flooding) with no validated demand yet.

## Consequences

- SignalR needs a custom `IUserIdProvider` for the first time in this codebase - required regardless of any other choice here, since notifications are the first per-user-private real-time data.
- `NotificationPreference` must be checked on every notification-creation path (the new interceptor and the three new membership handlers), or a muted trigger type will still write rows.
- Mentions in Description, Acceptance Criteria, and wiki page body content remain visually functional (the editor already supports them) but silently produce no notification - an intentional v1 scope cut, not an oversight, since those fields are edited in place with commit-on-blur and have no discrete "authored" moment the way a comment does.
